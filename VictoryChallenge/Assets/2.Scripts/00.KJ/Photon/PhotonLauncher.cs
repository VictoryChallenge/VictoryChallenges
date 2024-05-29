using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace VictoryChallenge.KJ.Photon
{
    public class PhotonLauncher : MonoBehaviourPunCallbacks
    {
        public static PhotonLauncher Instance;      // �̱���

        /* Room */
        [SerializeField] TMP_InputField roomNameInputField;     // �� �̸� �Է� �ʵ�
        [SerializeField] TMP_Text roomNameText;                 // �� �̸� ǥ�� �ؽ�Ʈ
        [SerializeField] Transform roomListcontent;             // �� ���
        [SerializeField] GameObject roomListPrefab;             // �� ������

        /* Player */
        [SerializeField] Transform playerListContent;           // �÷��̾� ���
        [SerializeField] GameObject playerListPrefab;           // �÷��̾� ������

        /* Game Start */
        [SerializeField] GameObject startGameButton;            // ���� ���� ��ư

        /* Error Message */
        [SerializeField] TMP_Text errorText;                    // ���� ǥ�� �ؽ�Ʈ

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Debug.Log("����");
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("���� ����");
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {
            MenuManager.Instance.OpenMenu("title");
            Debug.Log("�κ� ����");
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
        }

        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
            {
                return;
            }

            PhotonNetwork.CreateRoom(roomNameInputField.text);
            MenuManager.Instance.OpenMenu("loading");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            errorText.text = "Room Creation Failed" + message;
            MenuManager.Instance.OpenMenu("error");
        }

        public void JoinRoom(RoomInfo info)
        {
            PhotonNetwork.JoinRoom(info.Name);
        }

    }

}
