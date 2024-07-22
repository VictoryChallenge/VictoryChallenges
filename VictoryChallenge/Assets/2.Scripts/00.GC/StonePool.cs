using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;

public class StonePool : MonoBehaviourPun
{
    [System.Serializable]
    public class Stone
    {
        public string tag;
        public GameObject prefeb;
        public int size;
    }

    public static StonePool SP;

    [SerializeField] public BoxCollider spawnCollider;

    public List<Stone> stones;
    public Dictionary<string, Queue<GameObject>> poolDic;
    private Dictionary<GameObject, Vector3> _initPos;

    private void Awake()
    {
       SP = this;
    }

    private void Start()
    {
        _initPos = new Dictionary<GameObject, Vector3>();

        if (PhotonNetwork.IsMasterClient)
        {
            InitPoolInstantiate();
            StartCoroutine(StoneRoutine(1.5f));
        }
    }

    public void InitPoolInstantiate()
    {
        poolDic = new Dictionary<string, Queue<GameObject>>();

        foreach (Stone stone in stones)
        {
            Queue<GameObject> objpool = new Queue<GameObject>();
            for (int i = 0; i < stone.size; i++)
            {
                GameObject obj = PhotonNetwork.Instantiate(stone.tag, transform.position, Quaternion.identity);
                _initPos[obj] = transform.position;
                obj.GetComponent<PhotonView>().RPC("SetActiveRPC", RpcTarget.AllBuffered, false);
                objpool.Enqueue(obj);
            }
            poolDic.Add(stone.tag, objpool);
        }
    }

    public GameObject PoolInstantiate(string tag, Vector3 position, Quaternion rotation)
    {
        if(!poolDic.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} dosen't excist");
            return null;
        }

        GameObject obj = poolDic[tag].Dequeue();
        obj.GetComponent<PhotonView>().RPC("SetActiveRPC", RpcTarget.AllBuffered, true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        poolDic[tag].Enqueue(obj);

        return obj;
    }

    public void PoolDestroy(GameObject stone)
    {
        stone.transform.position = _initPos[stone];
        stone.GetComponent<PhotonView>().RPC("SetActiveRPC", RpcTarget.AllBuffered, false);
        
    }

    
    private Vector3 RandomPosition()
    {
        if (spawnCollider != null)
        {
            Bounds bounds = spawnCollider.bounds;
            Vector3 randPosition = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                                               Random.Range(bounds.min.y, bounds.max.y),
                                               Random.Range(bounds.min.z, bounds.max.z));

            return randPosition;
        }

        return Vector3.zero;
    }

    private IEnumerator StoneRoutine(float interval)
    {
        while (true)
        {
            Vector3 position = RandomPosition();
            GameObject stone = PoolInstantiate("Stone", position, Quaternion.identity);
        
            if(stone != null)
                StartCoroutine(DeActiveStone(stone, 8f));
            
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator DeActiveStone(GameObject stone, float delay)
    {
        yield return new WaitForSeconds(delay);
        PoolDestroy(stone);
    }

    
}