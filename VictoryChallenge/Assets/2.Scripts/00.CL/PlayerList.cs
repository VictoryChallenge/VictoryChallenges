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
                Debug.Log("��ŸƮ�� �÷��̾� ����Ʈ ������Ʈ");
                Invoke("UpdatePlayerList", 0.25f);
            }
        }

        public override void OnJoinedRoom()
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                Debug.Log("onjoinedroom �÷��̾� ����Ʈ ������Ʈ");
                UpdatePlayerList();
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("onplayerenteredroom �÷��̾� ����Ʈ ������Ʈ");

            GameObject prefab = newPlayer.NickName == PhotonNetwork.NickName ? playerListPrefab1 : playerListPrefab2;
            PlayerListItem listItem = Instantiate(prefab, playerListContent).GetComponent<PlayerListItem>();
            listItem.SetUp(newPlayer);

            //// 0.25�� �Ŀ� Refresh �Լ��� ȣ��
            //Invoke(nameof(CallRefresh), 0.4f);
        }

        // CallRefresh �޼��� �߰�
        private void CallRefresh()
        {
            // playerListContent ���� ��� PlayerListItem�� ���� Refresh ȣ��
            foreach (Transform child in playerListContent)
            {
                PlayerListItem item = child.GetComponent<PlayerListItem>();
                if (item != null)
                {
                    item.Refresh();
                }
            }
        }

        public void UpdatePlayerList()
        {
            Player[] players = PhotonNetwork.PlayerList;

            // ���� ����Ʈ Ŭ����
            foreach (Transform child in playerListContent)
            {
                Destroy(child.gameObject);
            }

            // �÷��̾� ����Ʈ ������Ʈ
            foreach (Player player in players)
            {
                GameObject prefab = player.NickName == PhotonNetwork.NickName ? playerListPrefab1 : playerListPrefab2;
                PlayerListItem item = Instantiate(prefab, playerListContent).GetComponent<PlayerListItem>();
                item.SetUp(player);
            }
        }

        public void ReadyPlayer()
        {
            foreach (Transform child in playerListContent)
            {
               child.GetComponent<PlayerListItem>().Refresh();
            }
        }
    }
}
