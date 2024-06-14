using UnityEngine;

namespace VictoryChallenge.KJ.Spawn
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;

        [SerializeField] SpawnPoint[] spawnPoints;

        void Awake()
        {
            Instance = this;
            spawnPoints = GetComponentsInChildren<SpawnPoint>();
        }

        public Transform GetSpawnPoint()
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
        }
    }

}
