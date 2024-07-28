using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class ObstacleManager : MonoBehaviourPun
    {
        public GameObject[] obstaclePrefabs;
        public GameObject obstacleSpawnEffectRed;
        public GameObject obstacleSpawnEffectBlue;
        public Transform spawnPointRed;
        public Transform spawnPointBlue;
        public float spawnInterval = 1f;
        public bool obstaclespawn = false;

        //private PhotonView photonView;

        public IEnumerator SpawnObstacles()
        {
            while (obstaclespawn)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    int randomIndexRed = Random.Range(0, obstaclePrefabs.Length);
                    int randomIndexBlue = Random.Range(0, obstaclePrefabs.Length);
                    photonView.RPC("SpawnRandomObstacles", RpcTarget.All, randomIndexRed, randomIndexBlue);
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        [PunRPC]
        private void SpawnRandomObstacles(int randomIndexRed, int randomIndexBlue)
        {
            Debug.Log($"SyncSpawnObstacles called with indices: {randomIndexRed}, {randomIndexBlue}");

            GameObject selectedPrefabRed = obstaclePrefabs[randomIndexRed];
            Instantiate(selectedPrefabRed, spawnPointRed.position, spawnPointRed.rotation);
            SpawnEffectRed(spawnPointRed.position, spawnPointRed.rotation);

            GameObject selectPrefabBlue = obstaclePrefabs[randomIndexBlue];
            Instantiate(selectPrefabBlue, spawnPointBlue.position, spawnPointBlue.rotation);
            SpawnEffectBlue(spawnPointBlue.position, spawnPointBlue.rotation);
        }

        private void SpawnEffectRed(Vector3 position, Quaternion rotation)
        {
            if (obstacleSpawnEffectRed != null)
            {
                Instantiate(obstacleSpawnEffectRed, position, rotation);
                Debug.Log("���� ����Ʈ ����");
            }
            else
            {
                Debug.Log("������ ����Ʈ ����");
            }
        }

        private void SpawnEffectBlue(Vector3 position, Quaternion rotation)
        {
            if (obstacleSpawnEffectBlue != null)
            {
                Instantiate(obstacleSpawnEffectBlue, position, rotation);
                Debug.Log("��� ����Ʈ ����");
            }
            else
            {
                Debug.Log("�Ķ��� ����Ʈ ����");
            }
        }
    }

}