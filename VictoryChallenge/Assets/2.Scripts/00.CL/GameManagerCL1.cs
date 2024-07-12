using Photon.Pun;
using VictoryChallenge.KJ.Photon;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace VictoryChallenge.Scripts.CL
{
    public class GameManagerCL1 : MonoBehaviourPunCallbacks
    {
        public static GameManagerCL1 instance;
        public static Dictionary<string, int> playerRanks = new Dictionary<string, int>();
        public static int currentRound = 1;
        private const int maxRounds = 3;
        private bool isCountdownStarted = false;
        private float countdownTime = 10f;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            isCountdownStarted = false;
        }

        public override void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoading;
        }

        public override void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoading;
        }
        #region 스타트

        void OnSceneLoading(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"현재씬 : {scene.name}, 빌드인덱스: {scene.buildIndex}");

            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                Debug.Log("게임씬입니다.");
                // 플레이어의 씬 로드 상태를 CustomProperties에 설정
                Hashtable propers = new Hashtable
                {
                    { "SceneLoaded", true }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(propers);
                Debug.Log("커스텀프로퍼티 설정 완료");
            }

            foreach (DictionaryEntry entry in PhotonNetwork.LocalPlayer.CustomProperties)
            {
                Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");
            }

            Debug.Log("커스텀프로퍼티제대로됨");
        }

        // 마스터 클라이언트가 각 플레이어의 상태를 확인하고, 모든 플레이어가 씬을 로드했으면 게임을 시작합니다.
        [PunRPC]
        void PlayerLoadedScene()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("마스터 클라이언트 확인중");
                // 모든 플레이어가 씬을 로드했는지 확인
                if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("SceneLoaded") && (bool)player.CustomProperties["SceneLoaded"]))
                {
                    Debug.Log("모든 플레이어가 씬 로드 끝");
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
        #endregion

        #region 피니시

        public void OnPlayerFinish(string userId)
        {
            Debug.Log("결승선입니다");

            if (!playerRanks.ContainsKey(userId))
            {
                playerRanks[userId] = playerRanks.Count + 1; // 등수 매기기
                Hashtable props = new Hashtable
                {
                    { "PlayerRank", playerRanks[userId] }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                if (!isCountdownStarted)
                {
                    isCountdownStarted = true;
                    StartCoroutine(CountdownCoroutine());

                }
            }
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

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            Debug.Log("if문들어가기전");
            // 변경된 CustomProperties 확인
            if (changedProps.ContainsKey("SceneLoaded"))
            {
                Debug.Log("씬이동했으면 나와야지?");
                // SceneLoaded가 변경되었을 때만 RPC 호출
                photonView.RPC("PlayerLoadedScene", RpcTarget.All);
            }

            if (changedProps.ContainsKey("PlayerRank"))
            {
                string userId = targetPlayer.UserId;
                int rank = (int)changedProps["PlayerRank"];
                playerRanks[userId] = rank;
            }
        }

        private void EndGame()
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (!playerRanks.ContainsKey(player.UserId))
                {

                    playerRanks[player.UserId] = playerRanks.Count + 1;

                    Hashtable props = new Hashtable
                    {
                        { "PlayerRank", playerRanks[player.UserId] }
                    };
                    player.SetCustomProperties(props);
                }
            }

            // 결과 씬으로 이동
            SceneManager.LoadScene("VictoryCL");
        }

        public static void ResetGame()
        {
            playerRanks.Clear();
            currentRound = 1;
        }
    }
    #endregion 피니시
}
