using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;

namespace VictoryChallenge.KJ.Room
{
    public class RoomMananger : MonoBehaviourPunCallbacks
    {
        public static RoomMananger Instance;        // 싱글톤

        void Awake()
        {
            if (Instance)
            {
                Destroy(Instance);
                return;
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            Debug.Log($"Scene loaded: {scene.name}, Build Index: {scene.buildIndex}, In Room: {PhotonNetwork.InRoom}");
            if (scene.buildIndex == 1)
            {
                if (PhotonNetwork.InRoom)
                {
                    Debug.Log("플레이어 매니저 생성");
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
                }
            }
        }
    }
}
