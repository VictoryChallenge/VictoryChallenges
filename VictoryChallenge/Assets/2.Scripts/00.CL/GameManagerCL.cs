using Photon.Pun;
using ExitGames.Client.Photon;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VictoryChallenge.KJ.Photon;
using Photon.Realtime;
using VictoryChallenge.KJ.Database;
using ExitGames.Client.Photon.StructWrapping;
using VictoryChallenge.KJ.Room;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace VictoryChallenge.Scripts.CL
{
    // 이 스크립트는 모든 플레이어가 씬을 로드했는지 확인하고, 그 후 게임을 시작하는 역할을 합니다.
    public class GameManagerCL : MonoBehaviourPunCallbacks
    {
        private bool isGameStarted = false;

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
                    { "SceneLoaded", true },
                    { "IsGoaledIn", false }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            // 변경된 CustomProperties 확인
            //if (changedProps.ContainsKey("SceneLoaded"))
            //{
            //    // SceneLoaded가 변경되었을 때만 RPC 호출
            //    photonView.RPC("PlayerLoadedScene", RpcTarget.All);
            //}
            if (changedProps.ContainsKey("SceneLoaded"))
            {
                // SceneLoaded가 변경되었을 때만 RPC 호출
                if (PhotonNetwork.IsMasterClient)
                {
                    // 모든 플레이어가 씬을 로드했는지 확인
                    if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("SceneLoaded") && (bool)player.CustomProperties["SceneLoaded"]))
                    {
                        // 이미 게임이 시작되지 않았는지 확인
                        if (!isGameStarted)
                        {
                            isGameStarted = true;
                            double startTime = PhotonNetwork.Time + 0.1f;
                            photonView.RPC("StartGame", RpcTarget.All, startTime);
                        }
                    }
                }
            }

            if (changedProps.ContainsKey("IsGoaledIn"))
            {
                if (targetPlayer.CustomProperties.TryGetValue("IsGoaledIn", out bool debugBool))
                {
                    Debug.Log($"{targetPlayer}'s isGoaledIn is " + debugBool);
                }
            }
        }

        public void OnGoaledInCheck()
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsGoaledIn", true } });
        }

        public void ChooseFinalWinner()
        {
            foreach(var list in PhotonNetwork.PlayerList)
            {
                if(list.CustomProperties.ContainsKey("IsGoaledIn"))
                {
                    if(list.CustomProperties.TryGetValue("IsGoaledIn", out bool isCheck))
                    {
                        if(isCheck)
                        {
                            // 결승선에 들어온 경우
                            Debug.Log(list.NickName + " has Finish race, IsGoaledIn = " + isCheck);
                            SceneManager.LoadScene(5);
                        }
                        else
                        {
                            // 결승선에 들어오지 못한 경우
                            Debug.Log(list.NickName + " has not Finish race, IsGoaledIn = " + isCheck);
                            RoomMananger.Instance.LeaveRoom();
                            SceneManager.LoadScene(1);
                        }
                    }
                }
            }
        }

        //public void IntroCount()
        //{
        //    PlayerLoadedScene();
        //}

        [PunRPC]
        void StartGame(double startTime)
        {
            StartCoroutine(StartCountdown(startTime));
        }

        IEnumerator StartCountdown(double startTime)
        {
            double timeRemaining = startTime - PhotonNetwork.Time;
            while (timeRemaining > 0)
            {
                Debug.Log("Time remaining: " + timeRemaining);
                yield return new WaitForEndOfFrame();
                timeRemaining = startTime - PhotonNetwork.Time;
            }
            // 게임 시작 로직을 실행
            GameSceneUI gameSceneUI = FindObjectOfType<GameSceneUI>();
            if (gameSceneUI != null)
            {
                gameSceneUI.StartCoroutine("GameStart");
            }
        }
    }
}
