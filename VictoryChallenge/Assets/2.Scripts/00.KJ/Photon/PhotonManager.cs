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

        string gameVersion = "0.1.0";                       // ���� ���� ���� -> ���� �ٸ��� ��Ī X

        void Awake()
        {
            if (!PhotonNetwork.IsConnected)                 // ���ῡ ���������� �ٽ� ������
            {
                bool isConnected = PhotonNetwork.ConnectUsingSettings();
                Debug.Log("ConnectUsingSettings " + isConnected);
            }
        }
        #region Photon Connect

        public override void OnConnected()                  // ���ῡ ����
        {
            base.OnConnected();
            Debug.Log("OnConnected");
        }

        public override void OnConnectedToMaster()          // ������ ������ ����
        {
            base.OnConnectedToMaster();
            Debug.Log("OnConnectedToMaster");

            PhotonNetwork.GameVersion = gameVersion;    
            Debug.Log("���ӹ���" + PhotonNetwork.GameVersion);

            if (PhotonNetwork.GameVersion != gameVersion)   // ���� ������ �ٸ��� ���� ����
            {
                Debug.Log("Please GameVersion Update !");
                Application.Quit();
            }

            PhotonNetwork.AutomaticallySyncScene = true;    // ������(ȣ��Ʈ)�� ���� �ѱ�� Ŭ���̾�Ʈ�鵵 ���� �Ѿ

            MenuManager.Instance.OpenMenu("title");         // Ÿ��Ʋ �޴��� �̵�
        }
        #endregion

        #region Quick Match
        public void QuickMatch()                            // �� ��ġ
        {
            RoomOptions roomOptions = new RoomOptions()
            {
                IsVisible = false,      // ���� �κ񿡵� ���̰� ���� -> �� ��ġ�̹Ƿ� ������ ���� ����
                MaxPlayers = 8          // �ִ� �÷��̾� �ο� 8��
            };

            PhotonNetwork.JoinOrCreateRoom("QuickMatchRoom", roomOptions, TypedLobby.Default);
        }

        public override void OnJoinedRoom()                 // �濡 ������ ȣ��
        {
            base.OnJoinedRoom();

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(1);                 // �κ�(��) ������ �̵�
            }
        }
        #endregion

        #region Lobby
        public override void OnLeftRoom()                   // �κ�(��)���� �������� ȣ��
        {
            base.OnLeftRoom();                              
            CleanUpPhotonView();                            
            PhotonNetwork.LoadLevel(0);                     // �޴� ������ �̵�
        }

        private void LeaveRoom()
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

