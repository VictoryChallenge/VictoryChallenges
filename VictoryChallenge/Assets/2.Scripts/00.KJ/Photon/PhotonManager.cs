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
            Debug.Log("게임버전" + PhotonNetwork.GameVersion);

            if (!PhotonNetwork.IsConnected)                 // 연결에 실패했을시 다시 연결함
            {
                bool isConnected = PhotonNetwork.ConnectUsingSettings();
                Debug.Log("ConnectUsingSettings " + isConnected);
            }
        }
        #endregion

        

        #region Photon Connect

        public override void OnConnected()                  // 연결에 성공
        {
            base.OnConnected();
            Debug.Log("OnConnected");
        }

        public override void OnConnectedToMaster()          // 마스터 서버에 연결
        {
            base.OnConnectedToMaster();
            Debug.Log("OnConnectedToMaster");

            PhotonNetwork.AutomaticallySyncScene = true;    // 마스터(호스트)가 씬을 넘기면 클라이언트들도 같이 넘어감

            MenuManager.Instance.OpenMenu("title");         // 타이틀 메뉴로 이동
        }
        #endregion

        #region Quick Match
        public void QuickMatch()                            // 퀵 매치
        {
            RoomOptions roomOptions = new RoomOptions()
            {
                IsVisible = false,      // 방이 로비에도 보이게 설정 -> 퀵 매치이므로 보여질 이유 없음
                MaxPlayers = 4          // 최대 플레이어 인원 8명
            };

            PhotonNetwork.JoinOrCreateRoom("QuickMatchRoom", roomOptions, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(1);                 // 로비(방) 씬으로 이동
            }
        }
        #endregion

        public void OnApplicationQuit()
        {
            Application.Quit();
        }
    }
}

