using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePool : MonoBehaviour
{

    [SerializeField] private GameObject _stone;
    private int _poolSize = 10;
    [SerializeField] public BoxCollider spawnCollider;

    private Queue<GameObject> queue = new Queue<GameObject>();


    

    void Start()
    {

        for(int i = 0; i < _poolSize; i++)
        {
            GameObject stone = Instantiate(_stone);
            queue.Enqueue(stone);
            stone.SetActive(false);
        }

        StartCoroutine(StoneRoutine(1.5f));
    }

    public void InsertQueue(GameObject stone)
    {
        stone.SetActive(false);
        queue.Enqueue(stone);
        
    }

    public GameObject GetQueue()
    {
        if (queue.Count > 0)
        {
            GameObject Deque_Stone = queue.Dequeue();
            Deque_Stone.SetActive(true);
            Debug.Log("켜짐?");
            RandomPosition(Deque_Stone);
            return Deque_Stone;
        }
        else
        {
            GameObject stone = Instantiate(_stone);
            RandomPosition(stone);
            return stone;
        }
    }

    private void RandomPosition(GameObject obj)
    {
        if (spawnCollider != null)
        {
            Bounds bounds = spawnCollider.bounds;
            Vector3 randPosition = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                                               Random.Range(bounds.min.y, bounds.max.y),
                                               Random.Range(bounds.min.z, bounds.max.z));

            obj.transform.position = randPosition;
        }
        else
        {
            Debug.LogWarning("spawnAreaCollider가 설정되지 않았습니다.");
        }
    }

    private IEnumerator StoneRoutine(float interval)
    {
        while(true)
        {
            GameObject stone = GetQueue();
            StartCoroutine(DeActiveStone(stone, 10f));
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator DeActiveStone(GameObject stone, float delay)
    {
        yield return new WaitForSeconds(delay);
        InsertQueue(stone);
    }
}
