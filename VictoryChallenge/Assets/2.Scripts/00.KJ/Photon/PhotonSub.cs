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
using GSpawn;
using VictoryChallenge.KJ.Spawn;
using ExitGames.Client.Photon.StructWrapping;

namespace VictoryChallenge.KJ.Photon
{
    public class PhotonSub : MonoBehaviourPunCallbacks
    {

        [HideInInspector] public Button _button;
        [HideInInspector] public TMP_Text _text;

        [HideInInspector] public bool _isReady = false;
        [HideInInspector] public int stageNum = 3;

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

                // PhotonNetwork.LocalPlayer.ActorNumber�� 1���� ����
                int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; 
                Transform spawnPoint = SpawnManager.Instance.GetIndexSpawnPoint(playerIndex);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), spawnPoint.position, Quaternion.identity);
            }
            else if (SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6 || SceneManager.GetActiveScene().buildIndex == 9)
            {
                Debug.Log("Ŭ�� �÷��̾� �Ŵ��� ����");

                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PlayerNumber", out int checkNum))
                {
                    Debug.Log("checkNum = " + checkNum);
                    Transform spawnPoint = SpawnManager.Instance.GetIndexSpawnPoint(checkNum);
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), spawnPoint.position, Quaternion.identity);
                }
                else
                {
                    // PhotonNetwork.LocalPlayer.ActorNumber�� 1���� ����
                    int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
                    Transform spawnPoint = SpawnManager.Instance.GetIndexSpawnPoint(playerIndex);
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), spawnPoint.position, Quaternion.identity);
                    //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
                }
            }
            else if((SceneManager.GetActiveScene().name == "WinnerCL") || (SceneManager.GetActiveScene().name == "LoseCL"))
            {
                // ����� ó��
            }
        }

        public override void OnJoinedRoom()                     // �κ�(��)�� ������ ��
        {
            _isReady = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable());

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
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "IsReady", _isReady } });
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
            //������ ���� �������� Ȯ���ϰ�, ���� ���°� �ƴϸ� ���� ���·� ����
            object isReady;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("IsReady", out isReady) == false)
            {
                _isReady = true;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "IsReady", _isReady } });
                Debug.Log("������ �غ� ���°� �ƴϾ����Ƿ� �غ� ���·� ������");
            }

            if (AllPlayersReady())
            {
                Hashtable props = new Hashtable() { { "isGameStarted", true } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);

                Debug.Log("��� �÷��̾ �غ��, ���� ����");
                PhotonNetwork.LoadLevel(stageNum);
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            if (SceneManager.GetActiveScene().buildIndex == 2)
                UpdateButtonText();
        }
        #endregion

        public void SetStageNum(int _stageNum)
        {
            stageNum = _stageNum;
            Debug.Log("�������� ��ȣ ����: " + stageNum);
        }


        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            UpdatePlayerNumber();
        }

        private void UpdatePlayerNumber()
        {
            List<Player> players = new List<Player>(PhotonNetwork.PlayerList);
            players.Sort((p1, p2) => p1.ActorNumber.CompareTo(p2.ActorNumber));

            for (int i = 0; i < players.Count; i++)
            {
                Hashtable props = new Hashtable { { "PlayerNumber", i + 1 } };
                players[i].SetCustomProperties(props);
                Debug.Log($"num = {i + 1}");
            }
        }

        public override void OnLeftRoom()                   // �κ�(��)���� �������� ȣ��
        {
            base.OnLeftRoom();
        }
    }
}
