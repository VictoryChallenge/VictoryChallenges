using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using VictoryChallenge.KJ.Menu;

namespace VictoryChallenge.KJ.Photon
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {

        #region Singleton
        public static PhotonManager Instance
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

        void Awake()
        {
            PhotonNetwork.GameVersion = "0.1.0";
            Debug.Log("���ӹ���" + PhotonNetwork.GameVersion);

            if (!PhotonNetwork.IsConnected)                 // ���ῡ ���������� �ٽ� ������
            {
                bool isConnected = PhotonNetwork.ConnectUsingSettings();
                Debug.Log("ConnectUsingSettings " + isConnected);
            }
        }
        #endregion

        

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
                MaxPlayers = 4          // �ִ� �÷��̾� �ο� 8��
            };

            PhotonNetwork.JoinOrCreateRoom("QuickMatchRoom", roomOptions, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(1);                 // �κ�(��) ������ �̵�
            }
        }
        #endregion

        public void OnApplicationQuit()
        {
            Application.Quit();
        }
    }
}

