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
        public static PhotonLauncher Instance;      // 싱글톤

        /* Room */
        [SerializeField] TMP_InputField roomNameInputField;     // 방 이름 입력 필드
        [SerializeField] TMP_Text roomNameText;                 // 방 이름 표시 텍스트
        [SerializeField] Transform roomListcontent;             // 방 목록
        [SerializeField] GameObject roomListPrefab;             // 방 프리팹

        /* Player */
        [SerializeField] Transform playerListContent;           // 플레이어 목록
        [SerializeField] GameObject playerListPrefab;           // 플레이어 프리팹

        /* Game Start */
        [SerializeField] GameObject startGameButton;            // 게임 시작 버튼

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
            Debug.Log("연결 성공");
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {
            MenuManager.Instance.OpenMenu("title");
            Debug.Log("로비에 들어옴");
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
