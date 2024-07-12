using ExitGames.Client.Photon;
using Photon.Pun;

using UnityEngine;
using UnityEngine.SceneManagement;
using VictoryChallenge.KJ.Photon;

namespace VictoryChallenge.KJ.Room
{
    public class RoomMananger : MonoBehaviourPunCallbacks
    {
        public static RoomMananger Instance;        // �̱���

        void Awake()
        {
            if (Instance)
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
            if (scene.buildIndex == 2 && PhotonNetwork.InRoom)
            {
                //PhotonSub.Instance.AssignButtonAndText();

                if (PhotonSub.Instance != null)
                {
                    Debug.Log("ȣ��Ʈ ȣ��");
                    PhotonSub.Instance.OnSceneLoadedForAllPlayers();
                }
            }
            else if (scene.buildIndex == 3)
            {
                if (PhotonSub.Instance != null)
                {
                    Debug.Log("Ŭ���̾�Ʈ ȣ��");
                    PhotonSub.Instance.OnSceneLoadedForAllPlayers();
                }
            }
        }

        #region Lobby
        public override void OnLeftRoom()                   // �κ�(��)���� �������� ȣ��
        {
            base.OnLeftRoom();
            //CleanUpPhotonView();
            PhotonNetwork.LoadLevel(1);                     // �޴� ������ �̵�
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        void CleanUpPhotonView()                            // Photon ������Ʈ ����
        {
            foreach (PhotonView pv in FindObjectsOfType<PhotonView>())
            {
                if (pv.IsMine && pv.ViewID > 0)
                {
                    Destroy(pv.gameObject);
                }
            }
        }
        #endregion
    }
}
