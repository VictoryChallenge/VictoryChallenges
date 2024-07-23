using System.Collections;
using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class ObstacleManager : MonoBehaviour
    {
        public GameObject[] obstaclePrefabs;
        public Transform spawnPointRed;
        public Transform spawnPointBlue;
        public float spawnInterval = 1.5f;


        public IEnumerator SpawnObstacles()
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

}