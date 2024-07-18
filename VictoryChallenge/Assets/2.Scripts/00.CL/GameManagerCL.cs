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
    // �� ��ũ��Ʈ�� ��� �÷��̾ ���� �ε��ߴ��� Ȯ���ϰ�, �� �� ������ �����ϴ� ������ �մϴ�.
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
            //if (changedProps.ContainsKey("SceneLoaded"))
            //{
            //    // SceneLoaded�� ����Ǿ��� ���� RPC ȣ��
            //    photonView.RPC("PlayerLoadedScene", RpcTarget.All);
            //}
            if (changedProps.ContainsKey("SceneLoaded"))
            {
                // SceneLoaded�� ����Ǿ��� ���� RPC ȣ��
                if (PhotonNetwork.IsMasterClient)
                {
                    // ��� �÷��̾ ���� �ε��ߴ��� Ȯ��
                    if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("SceneLoaded") && (bool)player.CustomProperties["SceneLoaded"]))
                    {
                        // �̹� ������ ���۵��� �ʾҴ��� Ȯ��
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
            // ���� ���� ������ ����
            GameSceneUI gameSceneUI = FindObjectOfType<GameSceneUI>();
            if (gameSceneUI != null)
            {
                gameSceneUI.StartCoroutine("GameStart");
            }
        }
    }
}
