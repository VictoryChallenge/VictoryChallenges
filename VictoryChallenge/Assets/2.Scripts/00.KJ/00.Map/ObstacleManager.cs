using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class ObstacleManager : MonoBehaviourPun
    {
        public GameObject[] obstaclePrefabs;
        public Transform spawnPoint;
        public float spawnInterval = 1.5f;
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
            GameObject selectedPrefab = obstaclePrefabs[randomIndex];
            Instantiate(selectedPrefab, spawnPoint.position, selectedPrefab.transform.rotation);
        }
    }
}