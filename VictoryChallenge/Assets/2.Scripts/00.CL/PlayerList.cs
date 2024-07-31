using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VictoryChallenge.KJ.Lobby;

namespace VictoryChallenge.Scripts.CL
{
    public class PlayerList : MonoBehaviourPunCallbacks
    {
        public static PlayerList Instance;

        /* PlayerList */
        public Transform playerListContent;
        [HideInInspector] public GameObject playerListPrefab1;
        [HideInInspector] public GameObject playerListPrefab2;

        void Awake()
        {
            Instance = this;

            playerListPrefab1 = Resources.Load<PlayerListItem>($"Prefabs/{"PlayerListItem"}").gameObject;
            playerListPrefab2 = Resources.Load<PlayerListItem>($"Prefabs/{"PlayerListItem2"}").gameObject;

            if (playerListPrefab1 == null || playerListPrefab2 == null)
            {
                Debug.LogError("Failed to load player list prefabs.");
            }
        }

        void Start()
        {
            if (PhotonNetwork.InRoom)
            {
                UpdatePlayerList();
            }
        }

        public override void OnJoinedRoom()
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                UpdatePlayerList();
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            GameObject prefab = newPlayer.NickName == PhotonNetwork.NickName ? playerListPrefab1 : playerListPrefab2;
            Instantiate(prefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
        }

        public void UpdatePlayerList()
        {
            Player[] players = PhotonNetwork.PlayerList;

            // 기존 리스트 클리어
            foreach (Transform child in playerListContent)
            {
                Destroy(child.gameObject);
            }

            // 플레이어 리스트 업데이트
            foreach (Player player in players)
            {
                GameObject prefab = player.NickName == PhotonNetwork.NickName ? playerListPrefab1 : playerListPrefab2;
                Instantiate(prefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
            }
        }
    }
}
