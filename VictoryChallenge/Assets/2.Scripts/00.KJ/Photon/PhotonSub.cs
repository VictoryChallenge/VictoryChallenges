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
                Debug.Log("호스트 플레이어 매니저 생성");
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            }
            else if (SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6 || SceneManager.GetActiveScene().buildIndex == 7)
            {
                Debug.Log("클라 플레이어 매니저 생성");
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            }
        }

        public override void OnJoinedRoom()                     // 로비(룸)에 들어왔을 때
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                OnSceneLoadedForAllPlayers();
            }

            if (PhotonNetwork.IsMasterClient)
            {
                _isReady = true;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsReady", _isReady } });
                Debug.Log("호스트 준비 상태");
            }

            Debug.Log("유저 이름 " + PhotonNetwork.NickName);
        }

        public void UpdateButtonText()
        {
            Debug.Log("호스트인지 확인 " + PhotonNetwork.IsMasterClient);

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
            Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}" + (_isReady ? "준비완료" : "아직 준비완료 안함"));
            CheckAllPlayersReady();
        }

        private void CheckAllPlayersReady()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _button.interactable = AllPlayersReady();
                Debug.Log("모든 플레이어 준비 완료" + _button.interactable);
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
                        Debug.Log(player.NickName + "이(가) 아직 준비 안됨");
                        return false;
                    }
                    else
                    {
                        Debug.Log(player.NickName + "준비됨");
                    }
                }
                else
                {
                    Debug.Log(player.NickName + "이(가) 아직 준비 상태가 아님");
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
                Debug.Log("모든 플레이어가 준비됨, 게임 시작");
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
            Debug.Log("스테이지 번호 설정: " + stageNum);
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

            // 10초 후 게임 오버 처리
            EndGame();
        }

        private void EndGame()
        {
            // 모든 플레이어가 준비되었는지 확인
            if (!AllPlayersReady())
            {
                Debug.LogWarning("모든 플레이어가 준비되지 않았습니다. 결과 씬을 로드할 수 없습니다.");
                return;
            }

            // playerTimes 딕셔너리를 시간 순서로 정렬합니다.
            List<KeyValuePair<string, float>> sortedTimes = new List<KeyValuePair<string, float>>(playerTimes);
            sortedTimes.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value)); // 시간이 적게 걸린 순서로 정렬

            // 순위 부여 및 RPC를 통해 등수 정보를 모든 클라이언트에게 전달합니다.
            for (int i = 0; i < sortedTimes.Count; i++)
            {
                string userId = sortedTimes[i].Key;
                playerRanks[userId] = i + 1;
                photonView.RPC("UpdatePlayerRank", RpcTarget.All, userId, playerRanks[userId]);
            }

            // 결과 씬으로 이동
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
