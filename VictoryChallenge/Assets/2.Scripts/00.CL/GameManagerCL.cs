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
using VictoryChallenge.KJ.Room;

namespace VictoryChallenge.Scripts.CL
{
    // �� ��ũ��Ʈ�� ��� �÷��̾ ���� �ε��ߴ��� Ȯ���ϰ�, �� �� ������ �����ϴ� ������ �մϴ�.
    public class GameManagerCL : MonoBehaviourPunCallbacks
    {
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
                    { "IsGoaledIn", false }
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
                            // ��¼��� ���� ���
                            Debug.Log(list.NickName + " has Finish race, IsGoaledIn = " + isCheck);
                            SceneManager.LoadScene(5);
                        }
                        else
                        {
                            // ��¼��� ������ ���� ���
                            Debug.Log(list.NickName + " has not Finish race, IsGoaledIn = " + isCheck);
                            RoomMananger.Instance.LeaveRoom();
                            SceneManager.LoadScene(1);
                        }
                    }
                }
            }
        }

        public void IntroCount()
        {
            PlayerLoadedScene();
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
