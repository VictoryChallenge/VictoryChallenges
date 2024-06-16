//using Photon.Pun;
//using UnityEngine;

//namespace VictoryChallenge.KJ.Lobby
//{
//    public class LobbyManager : MonoBehaviourPunCallbacks
//    {

//        #region Singleton
//        public static LobbyManager Instance
//        {
//            get
//            {
//                if (_instance == null)
//                {
//                    _instance = new GameObject(typeof(PhotonNetwork).Name).AddComponent<LobbyManager>();
//                    DontDestroyOnLoad(_instance.gameObject);
//                }
//                return _instance;
//            }
//        }

//        private static LobbyManager _instance;

//        void Awake()
//        {
//            PhotonNetwork.GameVersion = "0.1.0";
//            Debug.Log("게임버전" + PhotonNetwork.GameVersion);

//            if (!PhotonNetwork.IsConnected)                 // 연결에 실패했을시 다시 연결함
//            {
//                bool isConnected = PhotonNetwork.ConnectUsingSettings();
//                Debug.Log("ConnectUsingSettings " + isConnected);
//            }
//        }
//        #endregion

//        #region Lobby
//        public override void OnLeftRoom()                   // 로비(룸)에서 떠났으면 호출
//        {
//            base.OnLeftRoom();
//            CleanUpPhotonView();
//            PhotonNetwork.LoadLevel(1);                     // 메뉴 씬으로 이동
//        }

//        public void LeaveRoom()
//        {
//            PhotonNetwork.LeaveRoom();
//        }

//        void CleanUpPhotonView()                            // Photon 오브젝트 삭제
//        {
//            foreach (PhotonView pv in FindObjectsOfType<PhotonView>())
//            {
//                if (pv.IsMine && pv.ViewID > 0)
//                {
//                    Destroy(pv.gameObject);
//                }
//            }
//        }
//        #endregion

//        //#region Team
//        //public void TeamSelectInRoom(int team)
//        //{
//        //    if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
//        //    {
//        //        PhotonNetwork.LocalPlayer.CustomProperties["Team"] = team;
//        //    }
//        //    else
//        //    {
//        //        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable
//        //        {
//        //            {"Team", team }
//        //        };

//        //        PhotonNetwork.SetPlayerCustomProperties(playerProps);
//        //    }
//        //}
//        //#endregion
//    }
//}

