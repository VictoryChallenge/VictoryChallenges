using ExitGames.Client.Photon;
using Photon.Pun;

using UnityEngine;
using UnityEngine.SceneManagement;
using VictoryChallenge.KJ.Photon;
using VictoryChallenge.Scripts.CL;

namespace VictoryChallenge.KJ.Room
{
    public class RoomMananger : MonoBehaviourPunCallbacks
    {
        public static RoomMananger Instance;        // 싱글톤

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
                    Debug.Log("호스트 호출");
                    PhotonSub.Instance.OnSceneLoadedForAllPlayers();
                }
            }
            else if (scene.buildIndex >= 3 && scene.buildIndex != 4)
            {

                if (PhotonSub.Instance != null)
                {
                    Debug.Log("클라이언트 호출");
                    PhotonSub.Instance.OnSceneLoadedForAllPlayers();
                }
            }
        }

        #region Lobby
        public override void OnLeftRoom()                   // 로비(룸)에서 떠났으면 호출
        {
            base.OnLeftRoom();
            //CleanUpPhotonView();
            PhotonNetwork.LoadLevel(1);                     // 메뉴 씬으로 이동
            Scripts.CL.Managers.Sound.Play("MainBGM", Define.Sound.BGM);
        }

        public void LeaveRoom()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                Debug.LogWarning("Client is not connected or not in a room.");
            }
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
