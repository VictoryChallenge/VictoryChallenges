using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class ObstacleManager : MonoBehaviourPun
    {
        public GameObject[] obstaclePrefabs;
        public GameObject obstacleSpawnEffect;
        public Transform spawnPoint;
        public float spawnInterval = 1f;
        public bool obstaclespawn = false;

        public IEnumerator SpawnObstacles()
        {
            while (obstaclespawn)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    int randomIndex = Random.Range(0, obstaclePrefabs.Length);
                    photonView.RPC("SpawnRandomObstacles", RpcTarget.All, randomIndex);
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        [PunRPC]
        private void SpawnRandomObstacles(int randomIndex)
        {
            GameObject selectedPrefabRed = obstaclePrefabs[randomIndex];
            Instantiate(selectedPrefabRed, spawnPoint.position, spawnPoint.rotation);
            SpawnEffectRed(spawnPoint.position, spawnPoint.rotation);
        }

        private void SpawnEffectRed(Vector3 position, Quaternion rotation)
        {
            if (obstacleSpawnEffect != null)
            {
                Instantiate(obstacleSpawnEffect, position, rotation);
                Debug.Log("·¹µå ÀÌÆåÆ® »ý¼º");
            }
            else
            {
                Debug.Log("»¡°£»ö ÀÌÆåÆ® ¾øÀ½");
            }
        }
    }
}