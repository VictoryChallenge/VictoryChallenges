using System.Collections;
using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class ObstacleManager : MonoBehaviour
    {
        public GameObject[] obstaclePrefabs;
        public Transform spawnPointRed;
        public Transform spawnPointBlue;
        public float spawnInterval = 1f;


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
            int randomIndexRed = Random.Range(0, obstaclePrefabs.Length);
            GameObject selectedPrefabRed = obstaclePrefabs[randomIndexRed];
            Instantiate(selectedPrefabRed, spawnPointRed.position, spawnPointRed.rotation);

            int randomIndexBlue = Random.Range(0, obstaclePrefabs.Length);
            GameObject selectPrefabBlue = obstaclePrefabs[randomIndexBlue];
            Instantiate(selectPrefabBlue, spawnPointBlue.position, spawnPointBlue.rotation);
        }
    }

}