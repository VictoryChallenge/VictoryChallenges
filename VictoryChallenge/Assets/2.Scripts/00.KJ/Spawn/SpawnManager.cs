using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using VictoryChallenge.KJ.Photon;

namespace VictoryChallenge.KJ.Spawn
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;

        private static int _pointLength;
        [SerializeField] SpawnPoint[] spawnPoints;

        void Awake()
        {
            Instance = this;
            int a = PhotonSub.Instance.stageNum;

            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 2:
                    // ·Îºñ
                    _pointLength = 1;
                    break;
                case 3:
                case 5:
                case 6:
                case 7:
                    // Just Run
                    _pointLength = 4;
                    break;
                default:
                    _pointLength = 0;
                    break;
            }
            spawnPoints = new SpawnPoint[_pointLength];
            Debug.Log("Count : " + spawnPoints.Length);

            spawnPoints = this.gameObject.GetComponentsInChildren<SpawnPoint>();
            Debug.Log("current scene index " + SceneManager.GetActiveScene().buildIndex);
        }

        public Transform GetSpawnPoint()
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
        }
    }
}


//if (SceneManager.GetActiveScene().buildIndex == 2)