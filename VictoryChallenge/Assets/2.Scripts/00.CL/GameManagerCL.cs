using Photon.Pun;
using ExitGames.Client.Photon;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VictoryChallenge.KJ.Photon;
using Photon.Realtime;

namespace VictoryChallenge.Scripts.CL
{
    public class GameManagerCL : MonoBehaviourPunCallbacks
    {
        // 이 스크립트는 모든 플레이어가 씬을 로드했는지 확인하고, 그 후 게임을 시작하는 역할을 합니다.

        private void Start()
        {
            OnSceneLoaded();
        }

        void OnSceneLoaded()
        {
            if (SceneManager.GetActiveScene().buildIndex >= 3 && SceneManager.GetActiveScene().buildIndex != 4)
            {
                Debug.Log("ㅇㅇㅇㅋ제[발");
                // 플레이어의 씬 로드 상태를 CustomProperties에 설정
                Hashtable props = new Hashtable
                {
                    { "SceneLoaded", true }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            // 변경된 CustomProperties 확인
            if (changedProps.ContainsKey("SceneLoaded"))
            {
                // SceneLoaded가 변경되었을 때만 RPC 호출
                photonView.RPC("PlayerLoadedScene", RpcTarget.All);
            }
        }       

        // 마스터 클라이언트가 각 플레이어의 상태를 확인하고, 모든 플레이어가 씬을 로드했으면 게임을 시작합니다.
        [PunRPC]
        void PlayerLoadedScene()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("zzz마스터클");
                // 모든 플레이어가 씬을 로드했는지 확인
                if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("SceneLoaded") && (bool)player.CustomProperties["SceneLoaded"]))
                {
                    Debug.Log("왜안나오는데왜왜왜왜ㅗ애ㅗ애ㅗ애");
                    // 모든 플레이어가 씬을 로드했음을 알리는 RPC 호출
                    photonView.RPC("StartGame", RpcTarget.All);
                }
            }
        }

        [PunRPC]
        void StartGame()
        {
            // 게임 시작 로직을 실행
            GameSceneUI gameSceneUI = FindObjectOfType<GameSceneUI>();
            if (gameSceneUI != null)
            {
                gameSceneUI.StartCoroutine("GameStart");
            }
        }
    }
}
