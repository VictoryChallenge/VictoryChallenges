using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;

namespace VictoryChallenge.KJ.Room
{
    public class RoomMananger : MonoBehaviourPunCallbacks
    {
        public static RoomMananger Instance;        // �̱���

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
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
                    Debug.Log("�÷��̾� �Ŵ��� ����");
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
                }
            }
        }
    }
}
