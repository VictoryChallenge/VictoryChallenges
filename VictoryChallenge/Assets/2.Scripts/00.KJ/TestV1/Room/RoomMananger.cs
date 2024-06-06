using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;

namespace VictoryChallenge.KJ.Room
{
    public class RoomMananger : MonoBehaviourPunCallbacks
    {
        public static RoomMananger Instance;        // ΩÃ±€≈Ê

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
            if (scene.buildIndex == 1)
            {
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
                }
            }
        }
    }
}
