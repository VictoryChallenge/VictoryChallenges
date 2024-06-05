using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using VictoryChallenge.KJ.Menu;
using VictoryChallenge.KJ.Room;

namespace VictoryChallenge.KJ.Photon
{
    public class PhotonLauncher : MonoBehaviourPunCallbacks
    {
        //public static PhotonLauncher Instance;      // �̱���

        #region Singleton
        public static PhotonLauncher instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(PhotonNetwork).Name).AddComponent<PhotonLauncher>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        private static PhotonLauncher _instance;
        #endregion

        /* Room */
        [SerializeField] TMP_InputField roomNameInputField;     // �� �̸� �Է� �ʵ�
        [SerializeField] Transform roomListcontent;             // �� ���
        [SerializeField] GameObject roomListPrefab;             // �� ������

        /* Error Message */
        [SerializeField] TMP_Text errorText;                    // ���� ǥ�� �ؽ�Ʈ

        void Awake()
        {
            if (PhotonNetwork.IsConnected == false)
            {
                bool isConnected = PhotonNetwork.ConnectUsingSettings();
                Debug.Log("ConnectUsingSettings");
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster");
            base.OnConnected();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("�κ�");
            base.OnJoinedLobby();
            Debug.Log(PhotonNetwork.NickName);
            Menu.MenuManager2.Instance.OpenMenu("title");
            //PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
        }

        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
            {
                return;
            }

            Debug.Log("�� ���� �õ�");
            PhotonNetwork.CreateRoom(roomNameInputField.text);
            Menu.MenuManager2.Instance.OpenMenu("loading");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            errorText.text = "Room Creation Failed" + message;
            Menu.MenuManager2.Instance.OpenMenu("error");
        }

        public void JoinRoom(RoomInfo info)
        {
            Debug.Log("�� ���� " + info.Name);
            PhotonNetwork.JoinRoom(info.Name);
            Menu.MenuManager2.Instance.OpenMenu("loading");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("�� ���� ��");
            MenuManager2.Instance.OpenMenu("loading");
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
