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

        string gameVersion = "0.1.0";                       // 게임 버전 설정 -> 버전 다르면 매칭 X

        void Awake()
        {
            if (!PhotonNetwork.IsConnected)                 // 연결에 실패했을시 다시 연결함
            {
                bool isConnected = PhotonNetwork.ConnectUsingSettings();
                Debug.Log("ConnectUsingSettings " + isConnected);
            }
        }
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

            PhotonNetwork.GameVersion = gameVersion;    
            Debug.Log("게임버전" + PhotonNetwork.GameVersion);

            if (PhotonNetwork.GameVersion != gameVersion)   // 게임 버전이 다르면 게임 종료
            {
                Debug.Log("Please GameVersion Update !");
                Application.Quit();
            }

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
                MaxPlayers = 8          // 최대 플레이어 인원 8명
            };

            PhotonNetwork.JoinOrCreateRoom("QuickMatchRoom", roomOptions, TypedLobby.Default);
        }

        public override void OnJoinedRoom()                 // 방에 들어오면 호출
        {
            base.OnJoinedRoom();

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(1);                 // 로비(방) 씬으로 이동
            }
        }
        #endregion

        #region Lobby
        public override void OnLeftRoom()                   // 로비(룸)에서 떠났으면 호출
        {
            base.OnLeftRoom();                              
            CleanUpPhotonView();                            
            PhotonNetwork.LoadLevel(0);                     // 메뉴 씬으로 이동
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        void CleanUpPhotonView()                            // Photon 오브젝트 삭제
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

