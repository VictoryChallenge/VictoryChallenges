using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using TMPro; 
using UnityEngine; 
using UnityEngine.UI; 

namespace VictoryChallenge.Scripts.CL
{
    /// <summary>
    /// 채팅 기능을 관리하며, 메시지 송수신, 채팅 로그 표시, 개인 메시지 처리를 포함하는 클래스
    /// </summary>
    public class ChatManager : MonoBehaviour, IChatClientListener
    {
        public TMP_InputField inputField; 
        public TextMeshProUGUI chatDisplay; // 채팅 메시지 표시 텍스트
        public ScrollRect scrollRect; // 스크롤 뷰
        public ChatClient chatClient; // 채팅 클라이언트
        public string lastMessages; // 마지막 메시지
        string roomName; // 방 이름

        private List<string> chatMessages = new List<string>(); // 채팅 메시지 리스트
        private string userName; // 사용자 이름

        void Start()
        {
            PhotonNetwork.IsMessageQueueRunning = true; // 메시지 큐 실행

            InitializeChat(PhotonNetwork.NickName); // 채팅 초기화
        }

        void Update()
        {
            if (chatClient != null)
            {
                chatClient.Service(); // 채팅 클라이언트 서비스 실행
            }

            // 인풋 필드가 포커스를 받지 않은 상태에서 Enter 키를 누르면 인풋 필드 활성화 및 선택
            if (!inputField.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                inputField.ActivateInputField();
                inputField.Select();
            }

            // 인풋 필드가 비어 있지 않고 Enter 키를 누르면 메시지 전송
            if (!string.IsNullOrEmpty(inputField.text) && Input.GetKeyDown(KeyCode.Return))
            {
                SendMessage();
            }
        }

        /// <summary>
        /// 채팅을 초기화합니다.
        /// </summary>
        /// <param name="playerName">플레이어 이름</param>
        public void InitializeChat(string playerName)
        {
            userName = playerName; // 사용자 이름 설정
            chatClient = new ChatClient(this); // 채팅 클라이언트 초기화
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(userName)); // 채팅 서버에 연결
        }

        /// <summary>
        /// 채팅 관련 디버그 메시지를 출력합니다.
        /// </summary>
        public void DebugReturn(DebugLevel level, string message)
        {
            // 구현 없음
        }

        /// <summary>
        /// 채팅 상태 변경 시 호출됩니다.
        /// </summary>
        public void OnChatStateChange(ChatState state)
        {
            Debug.Log("Chat state changed to : " + state); // 새로운 채팅 상태를 출력
        }

        /// <summary>
        /// 채팅 서버에 연결되었을 때 호출됩니다.
        /// </summary>
        public void OnConnected()
        {
            chatClient.Subscribe(new string[] { "global" }); // 글로벌 채널에 가입, 로컬로도 가입 가능
            Debug.Log("Connected to chat"); // 연결 성공 메시지 출력
        }

        /// <summary>
        /// 채팅 서버에서 연결이 끊겼을 때 호출됩니다.
        /// </summary>
        public void OnDisconnected()
        {
            Debug.Log("Disconnected from chat"); // 연결 해제 메시지 출력
        }

        /// <summary>
        /// 채팅 메시지를 수신했을 때 호출됩니다.
        /// </summary>
        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            for (int i = 0; i < messages.Length; i++)
            {
                if (channelName == "global")
                {
                    chatMessages.Add($"{senders[i]} : {messages[i]}"); // 메시지를 리스트에 추가
                    lastMessages = messages[i].ToString(); // 마지막 메시지 업데이트
                }
            }
            UpdateChatDisplay(); // 채팅 디스플레이 업데이트
        }

        /// <summary>
        /// 개인 메시지를 수신했을 때 호출됩니다.
        /// </summary>
        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            if (sender != userName)
                chatDisplay.text += $"<color=blue>[귓속말] {sender} >> {message}</color>\n"; // 개인 메시지를 출력
        }

        /// <summary>
        /// 사용자 상태가 업데이트되었을 때 호출됩니다.
        /// </summary>
        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            // 구현 없음
        }

        /// <summary>
        /// 채널에 가입했을 때 호출됩니다.
        /// </summary>
        public void OnSubscribed(string[] channels, bool[] results)
        {
            Debug.Log("Subscribed to channels: " + string.Join(", ", channels)); // 가입한 채널들을 출력
        }

        /// <summary>
        /// 채널에서 탈퇴했을 때 호출됩니다.
        /// </summary>
        public void OnUnsubscribed(string[] channels)
        {
            // 구현 없음
        }

        /// <summary>
        /// 사용자가 채널에 가입했을 때 호출됩니다.
        /// </summary>
        public void OnUserSubscribed(string channel, string user)
        {
            // 구현 없음
        }

        /// <summary>
        /// 사용자가 채널에서 탈퇴했을 때 호출됩니다.
        /// </summary>
        public void OnUserUnsubscribed(string channel, string user)
        {
            // 구현 없음
        }

        /// <summary>
        /// 메시지를 전송합니다.
        /// </summary>
        public void SendMessage()
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                chatClient.PublishMessage("global", inputField.text); // 글로벌 채널에 메시지 전송
                inputField.text = ""; // 인풋 필드 초기화
            }
        }

        /// <summary>
        /// 채팅 디스플레이를 업데이트합니다.
        /// </summary>
        private void UpdateChatDisplay()
        {
            if (chatMessages.Count > 30)
            {
                chatMessages.RemoveRange(0, chatMessages.Count - 30); // 메시지가 30개를 초과하면 오래된 메시지 삭제
                Debug.Log("dd");
            }

            chatDisplay.text = string.Join("\n", chatMessages.ToArray()); // 채팅 메시지를 텍스트로 변환하여 표시

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f; // 스크롤을 가장 아래로 설정
        }

        /// <summary>
        /// 귓속말 명령어를 처리합니다.
        /// </summary>
        /// <param name="tokens">명령어 토큰</param>
        public void HandleWhisperCommand(string[] tokens)
        {
            // 명령어를 분석하여 대상 사용자 이름과 개인 메시지를 추출합니다.
            if (tokens.Length >= 3)
            {
                string targetUser = tokens[1]; // 대상 사용자 이름
                string message = string.Join(" ", tokens, 2, tokens.Length - 2); // 개인 메시지

                // 개인 메시지를 보냅니다.
                chatClient.SendPrivateMessage(targetUser, message, false);

                // 보낸 메시지를 채팅 디스플레이에 표시합니다.
                chatDisplay.text += $"<color=blue>[귓속말] {targetUser} << {message}</color>\n";
            }
            else
            {
                Debug.LogWarning("귓속말 명령어 형식이 올바르지 않습니다. /귓속말 대상사용자이름 메시지 or /귓말 대상사용자이름 메시지");
                chatDisplay.text += "<color=red>명령어가 올바르지 않습니다. /귓속말 대상사용자이름 메시지 or /귓말 대상사용자이름 메시지</color>\n";
            }
        }

        //private void ShowChatBubble(string sender, string message)
        //{
        //    // sender 이름으로 캐릭터를 찾기
        //    GameObject[] characters = GameObject.FindGameObjectsWithTag("Player"); // Assuming the player characters are tagged as "Player"
        //    foreach (var character in characters)
        //    {
        //        // Check if this character belongs to the sender
        //        if (character.name == sender)
        //        {
        //            Character charComponent = character.GetComponent<Character>();
        //            if (charComponent != null)
        //            {
        //                charComponent.photonView.RPC("ShowChatBubble", RpcTarget.All, message);
        //            }
        //            else
        //            {
        //                Debug.LogWarning("Character component not found in " + character.name);
        //            }
        //            break;
        //        }
        //    }
        //}
    }
}
