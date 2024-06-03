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
        public static PhotonLauncher Instance;      // 싱글톤

        /* Room */
        [SerializeField] TMP_InputField roomNameInputField;     // 방 이름 입력 필드
        [SerializeField] Transform roomListcontent;             // 방 목록
        [SerializeField] GameObject roomListPrefab;             // 방 프리팹

        /* Error Message */
        [SerializeField] TMP_Text errorText;                    // 에러 표시 텍스트

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Debug.Log("연결");
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster");
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {
            Menu.MenuManager.Instance.OpenMenu("title");
            Debug.Log("로비");
            Debug.Log(PhotonNetwork.NickName);
            //PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
        }

        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
            {
                return;
            }

            Debug.Log("방 생성 시도");
            PhotonNetwork.CreateRoom(roomNameInputField.text);
            Menu.MenuManager.Instance.OpenMenu("loading");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            errorText.text = "Room Creation Failed" + message;
            Menu.MenuManager.Instance.OpenMenu("error");
        }

        public void JoinRoom(RoomInfo info)
        {
            Debug.Log("방 진입 " + info.Name);
            PhotonNetwork.JoinRoom(info.Name);
            Menu.MenuManager.Instance.OpenMenu("loading");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("방 들어가는 중");
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
