using Photon.Pun;
using ExitGames.Client.Photon;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VictoryChallenge.KJ.Photon;
using Photon.Realtime;
using VictoryChallenge.KJ.Database;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections.Generic;
using VictoryChallenge.Scripts.HS;
using VictoryChallenge.Json.DataManage;

namespace VictoryChallenge.Scripts.CL
{
    public class GameManagerCL : MonoBehaviourPunCallbacks
    {
        // 이 스크립트는 모든 플레이어가 씬을 로드했는지 확인하고, 그 후 게임을 시작하는 역할을 합니다.
        Dictionary<string, int> playerRank = new Dictionary<string, int>();


        public enum Point
        {
            first = 10,
            second = 8,
            third = 6,
            fourth = 5,
            fifth = 4,
            sixth = 3,
            seventh = 2,
            eighth = 1,
        }

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
                    { "Score", 0 }
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

            int debugScore;
            if (changedProps.ContainsKey("Score"))
            {
                if (targetPlayer.CustomProperties.TryGetValue("Score", out debugScore))
                {
                    Debug.Log($"{targetPlayer}'s Score Changed " + debugScore);
                    RankManager.Instance.Register(targetPlayer.NickName, debugScore);
                }
            }
        }

        public void Register(string nickName, int rank)
        {
            playerRank.Add(nickName, rank);

            foreach (var item in playerRank.Keys)
            {
                Debug.Log("Nickname : " + item);
            }

            foreach (var item in playerRank.Values)
            {
                Debug.Log("rank : " + item);
            }
        }

        public int RewardPoint(int rank)
        {
            Point point;

            switch (rank)
            {
                case 1:
                    point = Point.first;
                    break;
                case 2:
                    point = Point.second;
                    break;
                case 3:
                    point = Point.third;
                    break;
                case 4:
                    point = Point.fourth;
                    break;
                case 5:
                    point = Point.fifth;
                    break;
                case 6:
                    point = Point.sixth;
                    break;
                case 7:
                    point = Point.seventh;
                    break;
                case 8:
                    point = Point.eighth;
                    break;
                default:
                    point = 0;
                    break;
            }

            //Debug.Log("point : " + (int)point);
            return (int)point;
        }

        public void SetRank(int rank)
        {
            int score = RewardPoint(rank);

            // 들어온 유저들의 닉네임 로그
            if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("Score")))
            {
                Debug.Log("PhotonNetwork.PlayerList : " + PhotonNetwork.PlayerList.ToStringFull());
            }

            // 결승선에 도달했을 때 Score 갱신
            // 문제점 -> PhotonNetwork.LocalPlayer.SetCustomProperties을 갱신하는 것이므로 다른 사람의 점수는 건드릴 수가 없다..?
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Score", score } });
        }

        public int ChooseWinner()
        {
            List<int> scores = new List<int>();

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if(player.CustomProperties.ContainsKey("Score"))
                {
                    scores.Add((int)player.CustomProperties["Score"]);
                    Debug.Log($"{player.NickName} is Add {(int)player.CustomProperties["Score"]}");
                }
            }

            scores.OrderByDescending(x => x).ToList();

            //foreach(var player in PhotonNetwork.PlayerList)
            //{
            //    if(player.CustomProperties.ContainsValue())
            //}
            //foreach(int score  in scores)
            //{
            //    Debug.Log("score in List : " + score);
            //}

            //Debug.Log("Sorted Score" + scores.First());

            return scores.First();
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
