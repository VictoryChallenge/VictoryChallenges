using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VictoryChallenge.KJ.Lobby
{
    public class LobbyLauncher : MonoBehaviourPunCallbacks
    {
        public static LobbyLauncher Instance;               // 싱글톤

        /* PlayerList */
        [SerializeField] Transform playerListContent;       // 플레이어 리스트
        [SerializeField] GameObject playerListPrefab;       // 플레이어 리스트 프리팹

        /* Game Start */
        [SerializeField] GameObject startGameButton;        // 게임 시작 버튼

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (PhotonNetwork.InRoom)
            {
                InitializeRoom();
            }
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("방에 성공적으로 들어옴");
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);

                if (Menu.MenuManager.Instance != null)
                {
                    Menu.MenuManager.Instance.OpenMenu("lobby");
                }
                InitializeRoom();
            }
        }

        public void InitializeRoom()
        {
            UpdatePlayerList();
            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }

        public void LeaveRoom()
        {
            Debug.Log("방을 떠나고 로비로 돌아감");
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            Debug.Log("메뉴로 돌아감");
            CleanUpPhotonView();
            PhotonNetwork.LoadLevel(0);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);   
        }

        public void UpdatePlayerList()
        {
            Player[] players = PhotonNetwork.PlayerList;

            foreach (Transform child in playerListContent)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < players.Count(); i++)
            {
                Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
            }
        }

        void CleanUpPhotonView()
        {
            foreach (PhotonView pv in FindObjectsOfType<PhotonView>())
            {
                if (pv.IsMine && pv.ViewID > 0)
                {
                    Destroy(pv.gameObject);
                }
            }
        }
    }
}
