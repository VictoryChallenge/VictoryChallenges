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

namespace VictoryChallenge.Scripts.CL
{
    public class GameManagerCL : MonoBehaviourPunCallbacks
    {
        // �� ��ũ��Ʈ�� ��� �÷��̾ ���� �ε��ߴ��� Ȯ���ϰ�, �� �� ������ �����ϴ� ������ �մϴ�.

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
                Debug.Log("����������[��");
                // �÷��̾��� �� �ε� ���¸� CustomProperties�� ����
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
            // ����� CustomProperties Ȯ��
            if (changedProps.ContainsKey("SceneLoaded"))
            {
                // SceneLoaded�� ����Ǿ��� ���� RPC ȣ��
                photonView.RPC("PlayerLoadedScene", RpcTarget.All);
            }

            int debugScore;
            if (changedProps.ContainsKey("Score"))
            {
                if (targetPlayer.CustomProperties.TryGetValue("Score", out debugScore))
                {
                    Debug.Log("Changed Score : " + debugScore);
                }
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

            Debug.Log($"Local NickName : {PhotonNetwork.LocalPlayer.NickName}" + "'s score : " + score);

            if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("Score")))
            {
                Debug.Log("PhotonNetwork.PlayerList : " + PhotonNetwork.PlayerList.ToStringFull());
            }
            
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
                }
            }

            scores.OrderByDescending(x => x).ToList();

            //foreach(var player in PhotonNetwork.PlayerList)
            //{
            //    if(player.CustomProperties.ContainsValue())
            //}
            foreach(int score  in scores)
            {
                Debug.Log("score in List : " + score);
            }

            Debug.Log("Sorted Score" + scores.First());

            return scores.First();
        }

        // ������ Ŭ���̾�Ʈ�� �� �÷��̾��� ���¸� Ȯ���ϰ�, ��� �÷��̾ ���� �ε������� ������ �����մϴ�.
        [PunRPC]
        void PlayerLoadedScene()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("zzz������Ŭ");
                // ��� �÷��̾ ���� �ε��ߴ��� Ȯ��
                if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("SceneLoaded") && (bool)player.CustomProperties["SceneLoaded"]))
                {
                    Debug.Log("�־ȳ����µ��ֿֿֿ֤Ǿ֤Ǿ֤Ǿ�");
                    // ��� �÷��̾ ���� �ε������� �˸��� RPC ȣ��
                    photonView.RPC("StartGame", RpcTarget.All);
                }
            }
        }

        [PunRPC]
        void StartGame()
        {
            // ���� ���� ������ ����
            GameSceneUI gameSceneUI = FindObjectOfType<GameSceneUI>();
            if (gameSceneUI != null)
            {
                gameSceneUI.StartCoroutine("GameStart");
            }
        }
    }
}
