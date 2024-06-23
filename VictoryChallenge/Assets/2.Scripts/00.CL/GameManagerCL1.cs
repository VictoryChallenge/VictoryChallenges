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
        #region ��ŸƮ

        void OnSceneLoading(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"����� : {scene.name}, �����ε���: {scene.buildIndex}");

            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                Debug.Log("���Ӿ��Դϴ�.");
                // �÷��̾��� �� �ε� ���¸� CustomProperties�� ����
                Hashtable propers = new Hashtable
                {
                    { "SceneLoaded", true }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(propers);
                Debug.Log("Ŀ����������Ƽ ���� �Ϸ�");
            }

            foreach (DictionaryEntry entry in PhotonNetwork.LocalPlayer.CustomProperties)
            {
                Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");
            }

            Debug.Log("Ŀ����������Ƽ����ε�");
        }

        // ������ Ŭ���̾�Ʈ�� �� �÷��̾��� ���¸� Ȯ���ϰ�, ��� �÷��̾ ���� �ε������� ������ �����մϴ�.
        [PunRPC]
        void PlayerLoadedScene()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("������ Ŭ���̾�Ʈ Ȯ����");
                // ��� �÷��̾ ���� �ε��ߴ��� Ȯ��
                if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("SceneLoaded") && (bool)player.CustomProperties["SceneLoaded"]))
                {
                    Debug.Log("��� �÷��̾ �� �ε� ��");
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
        #endregion

        #region �ǴϽ�

        public void OnPlayerFinish(string userId)
        {
            Debug.Log("��¼��Դϴ�");

            if (!playerRanks.ContainsKey(userId))
            {
                playerRanks[userId] = playerRanks.Count + 1; // ��� �ű��
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

            // 10�� �� ���� ���� ó��
            EndGame();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            Debug.Log("if��������");
            // ����� CustomProperties Ȯ��
            if (changedProps.ContainsKey("SceneLoaded"))
            {
                Debug.Log("���̵������� ���;���?");
                // SceneLoaded�� ����Ǿ��� ���� RPC ȣ��
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

            // ��� ������ �̵�
            SceneManager.LoadScene("VictoryCL");
        }

        public static void ResetGame()
        {
            playerRanks.Clear();
            currentRound = 1;
        }
    }
    #endregion �ǴϽ�
}
