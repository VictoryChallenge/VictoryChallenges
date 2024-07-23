using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using Photon.Realtime;
using System;
using VictoryChallenge.Scripts.CL;
using VictoryChallenge.Scripts.HS;
using System.Collections.Generic;
using VictoryChallenge.KJ.Database;
using Photon.Pun.Demo.Cockpit;

namespace VictoryChallenge.KJ.Photon
{
    public class PhotonSub : MonoBehaviourPunCallbacks
    {

        [HideInInspector] public Button _button;
        [HideInInspector] public TMP_Text _text;

        [HideInInspector] public bool _isReady = false;
        [HideInInspector] public int stageNum = 3;
        //private bool _isControllerCreated = false;


        public static Dictionary<string, int> playerRanks = new Dictionary<string, int>();
        public static Dictionary<string, float> playerTimes = new Dictionary<string, float>();
        private bool isCountdownStarted = false;
        private float countdownTime = 10f;
        public static int currentRound = 1;
        private const int maxRounds = 3;




        #region Singleton
        public static PhotonSub Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        #endregion

        #region Dynamic Buttons
        public void AssignButtonAndText()
        {
            _button = GameObject.Find("GameStart").GetComponent<Button>();
            _text = GameObject.Find("ReadyOrStart").GetComponent<TextMeshProUGUI>();
        }

        public void OnSceneLoadedForAllPlayers()
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                Debug.Log("ȣ��Ʈ �÷��̾� �Ŵ��� ����");
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            }
            else if (SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6 || SceneManager.GetActiveScene().buildIndex == 7)
            {
                Debug.Log("Ŭ�� �÷��̾� �Ŵ��� ����");
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            }
        }

        public override void OnJoinedRoom()                     // �κ�(��)�� ������ ��
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                OnSceneLoadedForAllPlayers();
            }

            if (PhotonNetwork.IsMasterClient)
            {
                _isReady = true;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsReady", _isReady } });
                Debug.Log("ȣ��Ʈ �غ� ����");
            }

            Debug.Log("���� �̸� " + PhotonNetwork.NickName);
        }

        public void UpdateButtonText()
        {
            Debug.Log("ȣ��Ʈ���� Ȯ�� " + PhotonNetwork.IsMasterClient);

            if (PhotonNetwork.IsMasterClient)
            {
                _text.text = "Game Start";
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(OnStartClicked);
            }
            else
            {
                _text.text = "Ready";
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(OnReadyClicked);
            }
        }

        public void OnReadyClicked()
        {
            _isReady = !_isReady;
            _text.text = _isReady ? "UnReady" : "Ready";
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsReady", _isReady } });
            Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}" + (_isReady ? "�غ�Ϸ�" : "���� �غ�Ϸ� ����"));
            CheckAllPlayersReady();
        }

        private void CheckAllPlayersReady()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _button.interactable = AllPlayersReady();
                Debug.Log("��� �÷��̾� �غ� �Ϸ�" + _button.interactable);
            }
        }

        private bool AllPlayersReady()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                object isReady;
                if (player.CustomProperties.TryGetValue("IsReady", out isReady))
                {
                    if (!(bool)isReady)
                    {
                        Debug.Log(player.NickName + "��(��) ���� �غ� �ȵ�");
                        return false;
                    }
                    else
                    {
                        Debug.Log(player.NickName + "�غ��");
                    }
                }
                else
                {
                    Debug.Log(player.NickName + "��(��) ���� �غ� ���°� �ƴ�");
                    return false;
                }
            }
            return true;
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            if (changedProps.ContainsKey("PlayerRank"))
            {
                string userId = targetPlayer.UserId;
                int rank = (int)changedProps["PlayerRank"];
                playerTimes[userId] = rank;
            }

            if (changedProps.ContainsKey("isReady"))
            {
                Debug.Log(targetPlayer.NickName + "Changed ready state to" + changedProps["IsReady"]);
                CheckAllPlayersReady();
            }
        }

        public void OnStartClicked()
        {
            if (PhotonNetwork.IsMasterClient && AllPlayersReady())
            {
                Debug.Log("��� �÷��̾ �غ��, ���� ����");
                PhotonNetwork.LoadLevel(stageNum);
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            UpdateButtonText();
        }
        #endregion

        public void SetStageNum(int _stageNum)
        {
            stageNum = _stageNum;
            Debug.Log("�������� ��ȣ ����: " + stageNum);
        }



        public void OnPlayerFinish(string userId, float finishTime)
        {
            if (!playerTimes.ContainsKey(userId))
            {
                playerTimes[userId] = finishTime;
                photonView.RPC("FinishMaster", RpcTarget.AllBuffered, userId, finishTime);

                if (!isCountdownStarted)
                {
                    StartCoroutine(CountdownCoroutine());
                }
            }
        }

        [PunRPC]
        public void FinishMaster(string userId, float finishTime)
        {
            playerTimes[userId] = finishTime;
        }

        private bool AllPlayersFinished()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!playerTimes.ContainsKey(player.UserId))
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerator CountdownCoroutine()
        {
            float remainingTime = countdownTime;
            while (remainingTime > 0)
            {
                Debug.Log($"Countdown: {remainingTime} seconds remaining");
                yield return new WaitForSeconds(1f);
                remainingTime -= 1f;
            }

            // 10�� �� ���� ���� ó��
            EndGame();
        }

        private void EndGame()
        {
            // ��� �÷��̾ �غ�Ǿ����� Ȯ��
            if (!AllPlayersReady())
            {
                Debug.LogWarning("��� �÷��̾ �غ���� �ʾҽ��ϴ�. ��� ���� �ε��� �� �����ϴ�.");
                return;
            }

            // playerTimes ��ųʸ��� �ð� ������ �����մϴ�.
            List<KeyValuePair<string, float>> sortedTimes = new List<KeyValuePair<string, float>>(playerTimes);
            sortedTimes.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value)); // �ð��� ���� �ɸ� ������ ����

            // ���� �ο� �� RPC�� ���� ��� ������ ��� Ŭ���̾�Ʈ���� �����մϴ�.
            for (int i = 0; i < sortedTimes.Count; i++)
            {
                string userId = sortedTimes[i].Key;
                playerRanks[userId] = i + 1;
                photonView.RPC("UpdatePlayerRank", RpcTarget.All, userId, playerRanks[userId]);
            }

            // ��� ������ �̵�
            PhotonNetwork.LoadLevel("VictoryCL");
        }


        [PunRPC]
        void UpdatePlayerRank(string userId, int rank)
        {
            playerRanks[userId] = rank;
        }

        public static void ResetGame()
        {
            playerTimes.Clear();
            playerRanks.Clear();
        }
    }
}
