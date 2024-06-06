using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using VictoryChallenge.KJ.Menu;

namespace VictoryChallenge.KJ.Photon
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        #region Singleton
        public static PhotonManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(PhotonNetwork).Name).AddComponent<PhotonManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        private static PhotonManager _instance;
        #endregion

        string gameVersion = "1";           // ���� ���� ���� -> ���� �ٸ��� ��Ī X

        void Awake()
        {
            if (PhotonNetwork.IsConnected == false)
            {
                bool isConnected = PhotonNetwork.ConnectUsingSettings();
                Debug.Log("ConnectUsingSettings " + PhotonNetwork.ConnectUsingSettings());
            }
        }
        #region Photon Connect
        public override void OnConnectedToMaster()
        {
            base.OnConnected();
            PhotonNetwork.GameVersion = gameVersion;
            Debug.Log(PhotonNetwork.GameVersion);

            if (PhotonNetwork.GameVersion != gameVersion)
            {
                Debug.Log("GameVersion Update !");
                Application.Quit();
            }

            PhotonNetwork.AutomaticallySyncScene = true;
            Debug.Log("OnConnectedToMaster ");

            MenuManager.Instance.OpenMenu("title");
        }
        #endregion

        #region Quick Match
        public void QuickMatch()
        {
            RoomOptions roomOptions = new RoomOptions()
            {
                IsVisible = false,      // ���� �κ񿡵� ���̰� ���� -> �� ��ġ�̹Ƿ� ������ ���� ����
                MaxPlayers = 8          // �ִ� �÷��̾� �ο� 8��
            };

            PhotonNetwork.JoinOrCreateRoom("QuickMatchRoom", roomOptions, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(1);
            }
        }
        #endregion

        #region Lobby
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            CleanUpPhotonView();
            PhotonNetwork.LoadLevel(0);
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        void CleanUpPhotonView()
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

