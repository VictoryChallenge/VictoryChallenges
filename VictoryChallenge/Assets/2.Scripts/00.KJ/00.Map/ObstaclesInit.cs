using System.Collections;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public Transform spawnPointRed;
    public Transform spawnPointBlue;
    public float spawnInterval = 2.5f;

    private Rigidbody rb;

    void Start()
    {
        StartCoroutine(SpawnObstacles());
        rb = GetComponent<Rigidbody>();
    }

    public void SetConveyorSpeed(Vector3 forceDirection, float speed)
    {
        rb.AddForce(forceDirection * speed);
    }

    private IEnumerator SpawnObstacles()
    {
        while (true)
        {
            SpawnRandomObstacles();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRandomObstacles()
    {
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject selectedPrefab = obstaclePrefabs[randomIndex];
        Instantiate(selectedPrefab, spawnPointRed.position, spawnPointRed.rotation);
        Instantiate(selectedPrefab, spawnPointBlue.position, spawnPointBlue.rotation);
    }
}
