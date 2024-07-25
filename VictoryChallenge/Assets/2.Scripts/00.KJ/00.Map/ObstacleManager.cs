using System.Collections;
using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class ObstacleManager : MonoBehaviour
    {
        public GameObject[] obstaclePrefabs;
        public GameObject obstacleSpawnEffectRed;
        public GameObject obstacleSpawnEffectBlue;
        public Transform spawnPointRed;
        public Transform spawnPointBlue;
        public float spawnInterval = 1f;
        public bool obstaclespawn = false;


        public IEnumerator SpawnObstacles()
        {
            while (obstaclespawn)
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
            SpawnEffectRed(spawnPointRed.position, spawnPointRed.rotation);

            int randomIndexBlue = Random.Range(0, obstaclePrefabs.Length);
            GameObject selectPrefabBlue = obstaclePrefabs[randomIndexBlue];
            Instantiate(selectPrefabBlue, spawnPointBlue.position, spawnPointBlue.rotation);
            SpawnEffectBlue(spawnPointBlue.position, spawnPointBlue.rotation);
        }

        private void SpawnEffectRed(Vector3 position, Quaternion rotation)
        {
            if (obstacleSpawnEffectRed != null)
            {
                Instantiate(obstacleSpawnEffectRed, position, rotation);
                Debug.Log("레드 이펙트 생성");
            }
            else
            {
                Debug.Log("빨간색 이펙트 없음");
            }
        }

        private void SpawnEffectBlue(Vector3 position, Quaternion rotation)
        {
            if (obstacleSpawnEffectBlue != null)
            {
                Instantiate(obstacleSpawnEffectBlue, position, rotation);
                Debug.Log("블루 이펙트 생성");
            }
            else
            {
                Debug.Log("파란색 이펙트 없음");
            }
        }
    }

}