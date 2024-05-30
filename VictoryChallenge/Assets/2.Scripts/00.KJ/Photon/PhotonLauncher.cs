using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VictoryChallenge.KJ.Menu;
using VictoryChallenge.KJ.Room;

namespace VictoryChallenge.KJ.Photon
{
    public class PhotonLauncher : MonoBehaviourPunCallbacks
    {
        public static PhotonLauncher Instance;      // �̱���

        /* Room */
        [SerializeField] TMP_InputField roomNameInputField;     // �� �̸� �Է� �ʵ�
        [SerializeField] Transform roomListcontent;             // �� ���
        [SerializeField] GameObject roomListPrefab;             // �� ������

        /* Error Message */
        [SerializeField] TMP_Text errorText;                    // ���� ǥ�� �ؽ�Ʈ

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Debug.Log("Start() ȣ���. ���� ���� Ȯ�� - PhotonNetwork.NetworkClientState: " + PhotonNetwork.NetworkClientState);
            if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
            {
                Debug.Log("PhotonNetwork.ConnectUsingSettings ȣ��");
                PhotonNetwork.ConnectUsingSettings();
            }
            else if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
            {
                Debug.Log("�̹� ������ ������ ����� ����, �κ�� ���� ����");
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster");
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {
            MenuManager.Instance.OpenMenu("title");
            Debug.Log("�κ�");
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
        }

        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
            {
                return;
            }

            Debug.Log("�� ���� �õ�");
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
            Debug.Log("�� ���� " + info.Name);
            PhotonNetwork.JoinRoom(info.Name);
            MenuManager.Instance.OpenMenu("loading");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("�� ���� ��");
            MenuManager.Instance.OpenMenu("loading");
            PhotonNetwork.LoadLevel(1);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (Transform trans in roomListcontent)
            {
                Destroy(trans.gameObject);
            }
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].RemovedFromList)
                    continue;
                Instantiate(roomListPrefab, roomListcontent).GetComponent<RoomListItem>().Setup(roomList[i]);
            }
        }
    }
}
