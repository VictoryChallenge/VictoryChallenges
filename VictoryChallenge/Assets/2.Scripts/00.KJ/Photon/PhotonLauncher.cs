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
            Debug.Log("Start() 호출됨. 연결 상태 확인 - PhotonNetwork.NetworkClientState: " + PhotonNetwork.NetworkClientState);
            if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
            {
                Debug.Log("PhotonNetwork.ConnectUsingSettings 호출");
                PhotonNetwork.ConnectUsingSettings();
            }
            else if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
            {
                Debug.Log("이미 마스터 서버에 연결된 상태, 로비로 직접 참가");
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
            Debug.Log("로비");
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
        }

        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
            {
                return;
            }

            Debug.Log("방 생성 시도");
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
            Debug.Log("방 진입 " + info.Name);
            PhotonNetwork.JoinRoom(info.Name);
            MenuManager.Instance.OpenMenu("loading");
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
