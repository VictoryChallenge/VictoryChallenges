using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VictoryChallenge.KJ.Lobby
{
    public class LobbyLauncher : MonoBehaviourPunCallbacks
    {
        public static LobbyLauncher Instance;               // �̱���

        /* PlayerList */
        [SerializeField] Transform playerListContent;       // �÷��̾� ����Ʈ
        [SerializeField] GameObject playerListPrefab;       // �÷��̾� ����Ʈ ������

        /* Game Start */
        [SerializeField] GameObject startGameButton;        // ���� ���� ��ư

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
            Debug.Log("�濡 ���������� ����");
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                Menu.MenuManager.Instance.OpenMenu("lobby");
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
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel(0);
        }

        public override void OnLeftRoom()
        {
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
    }
}
