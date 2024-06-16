using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 2:
                    // �κ�
                    _pointLength = 1;
                    break;
                case 3:
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