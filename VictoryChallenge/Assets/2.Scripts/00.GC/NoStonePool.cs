using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using characterController = VictoryChallenge.Controllers.Player.CharacterController;

public class NoStonePool : MonoBehaviourPun
{
    [System.Serializable]
    public class Stone
    {
        public string tag;
        public GameObject prefeb;
    }

    [SerializeField] public BoxCollider spawnCollider;
    public List<Stone> stones;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartStone", RpcTarget.All);
        }
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

    [PunRPC]
    public void StartStone()
    {
        StartCoroutine(StoneRoutine(1.5f));
    }
    
    private IEnumerator StoneRoutine(float interval)
    {
        yield return new WaitForSeconds(7f);

        while (true)
        {
            Vector3 position = RandomPosition();
            GameObject stone = PhotonNetwork.Instantiate("Stone", position, Quaternion.identity);

            if (stone != null)
            {
                StartCoroutine(DeActiveStone(stone, 20f));
            }

            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator DeActiveStone(GameObject stone, float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(stone);
    }


}
