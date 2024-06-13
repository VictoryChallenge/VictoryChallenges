using UnityEngine;
using VictoryChallenge.Customize;

namespace VictoryChallenge.Json
{
    public class CustomedPrefabsSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _customPrefab;
        [SerializeField] private Transform _spawnPos;
        [SerializeField] private PlayerCharacterCustomized _playerCharacterCustomized;


        private void Start()
        {
            
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                GameObject go = Instantiate(_customPrefab, _spawnPos.position, Quaternion.Euler(0, 180f, 0));
                PlayerCharacterCustomized custom = go.GetComponent<PlayerCharacterCustomized>();
                custom.Load();
            }
        }
    }
}
