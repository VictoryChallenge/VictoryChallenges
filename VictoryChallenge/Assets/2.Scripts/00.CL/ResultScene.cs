using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using VictoryChallenge.KJ.Photon;
using System.Collections;

namespace VictoryChallenge.Scripts.CL
{
    // 플레이어 정보 클래스 정의
    public class PlayerInfo
    {
        public string NickName;
        public string UserId;
    }

    public class ResultScene : MonoBehaviourPunCallbacks
    {
        public TextMeshProUGUI resultText;

        // Dictionary to map Player UserId to NickName
        private Dictionary<string, string> playerNicknames = new Dictionary<string, string>();
        private List<PlayerInfo> playersInfo = new List<PlayerInfo>();

        void Start()
        {
            // Check if the current client is the master client
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(PlayerNickNamesCoroutine());
            }
        }

        IEnumerator PlayerNickNamesCoroutine()
        {
            yield return new WaitForSeconds(0.5f);

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                PlayerInfo info = new PlayerInfo();
                info.NickName = player.NickName;
                info.UserId = player.UserId;
                playersInfo.Add(info);
            }

            // Sync player nicknames with all clients
            photonView.RPC("SyncPlayerNicknames", RpcTarget.All);
        }

        // 특정 UserId에 해당하는 플레이어의 닉네임을 반환하는 함수
        public string GetNicknameByUserId(string userId)
        {
            // 리스트에서 주어진 UserId와 일치하는 플레이어 정보를 찾음
            PlayerInfo playerInfo = playersInfo.Find(info => info.UserId == userId);

            // 찾은 플레이어 정보가 있으면 닉네임을 반환, 없으면 null 반환
            return playerInfo != null ? playerInfo.NickName : "unknown";
        }

        [PunRPC]
        void SyncPlayerNicknames()
        {
            DisplayResults(); // Display results after syncing nicknames
        }

        void DisplayResults()
        {
            var sortedRanks = PhotonSub.playerRanks.OrderByDescending(pair => pair.Value);
            string result = "Results:\n";

            foreach (var rank in sortedRanks)
            {
                string userId = rank.Key;
                string userName = GetNicknameByUserId(userId); // Retrieve nickname from dictionary
                result += $"UserID: {userName}, Rank: {rank.Value}\n";
            }

            resultText.text = result;
        }

        public void OnRestartButton()
        {
            PhotonSub.ResetGame();
            SceneManager.LoadScene("GameScene");
        }
    }
}
