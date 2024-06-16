using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine; 
using UnityEngine.UI;
using AuthenticationValues = Photon.Chat.AuthenticationValues;

namespace VictoryChallenge.Scripts.CL
{
    /// <summary>
    /// 채팅 기능을 관리하며, 메시지 송수신, 채팅 로그 표시, 개인 메시지 처리를 포함하는 클래스
    /// </summary>
    public class ChatManager : UI_Scene, IChatClientListener
    {
        enum Inputfields
        { 
            ChatInput,
        }

        enum TMPs
        { 
            ChatDisplay,
        }

        enum ScrollRects
        { 
            ChatScrollView,
        }

        TMP_InputField chatinput; 
        TextMeshProUGUI chatDisplay; // 채팅 메시지 표시 텍스트
        ScrollRect scrollRect; // 스크롤 뷰
        
        ChatClient chatClient; // 채팅 클라이언트
        string lastMessages; // 마지막 메시지
        string roomName; // 방 이름

        private List<string> chatMessages = new List<string>(); // 채팅 메시지 리스트
        private string userName; // 사용자 이름

        void Start()
        {
            Init();
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;

            PhotonNetwork.IsMessageQueueRunning = true; // 메시지 큐 실행

            InitializeChat(PhotonNetwork.NickName); // 채팅 초기화
            chatinput.onEndEdit.AddListener(OnInputEndEdit);
        }

        public override void Init()
        {
            base.Init();
            Bind<TMP_InputField>(typeof(Inputfields));
            Bind<TextMeshProUGUI>(typeof(TMPs));
            Bind<ScrollRect>(typeof(ScrollRects));

            chatinput = GetInputField((int)Inputfields.ChatInput);
            chatDisplay = GetTextMeshPro((int)TMPs.ChatDisplay);
            scrollRect = GetScrollRect((int)ScrollRects.ChatScrollView);
        }

        void Update()
        {
            if (chatClient != null)
            {
                chatClient.Service(); // 채팅 클라이언트 서비스 실행
            }

            // 인풋 필드가 포커스를 받지 않은 상태에서 Enter 키를 누르면 인풋 필드 활성화 및 선택
            if (!chatinput.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                chatinput.ActivateInputField();
                chatinput.Select();
            }

            // 인풋 필드가 비어 있지 않고 Enter 키를 누르면 메시지 전송
            if (!string.IsNullOrEmpty(chatinput.text) && Input.GetKeyDown(KeyCode.Return))
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
                if (channelName == "global" && !messages[i].ToString().StartsWith("/"))
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
            string senderId = null;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.NickName == sender)
                {
                    senderId = player.UserId; // 대상 사용자의 UID로 설정
                    break;
                }
            }

            if (sender != PhotonNetwork.NickName || senderId != PhotonNetwork.LocalPlayer.UserId)
            { 
                string whisperMessage = $"<color=blue>[귓속말] {sender} >> {message}</color>";
                chatMessages.Add(whisperMessage);
                UpdateChatDisplay();
            }
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

                // PhotonNetwork.PlayerList를 사용하여 대상 사용자의 UID를 찾습니다.
                string targetUserId = null;
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (player.NickName == targetUser)
                    {
                        targetUserId = player.UserId; // 대상 사용자의 UID로 설정
                        break;
                    }
                }

                if (targetUser == PhotonNetwork.NickName || targetUser == PhotonNetwork.LocalPlayer.UserId)
                {
                    string sendMeWarning = $"<color=red>자신에게는 메시지를 보낼 수 없습니다.</color>";
                    chatMessages.Add(sendMeWarning); // 경고 메시지를 리스트에 추가
                    UpdateChatDisplay(); // 채팅 디스플레이 업데이트
                    return;
                }

                // 대상 사용자가 온라인인지 확인
                bool targetOnline = false;
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (player.UserId == targetUser || player.NickName == targetUser)
                    {
                        targetOnline = true;
                        break;
                    }
                }

                if (!targetOnline)
                {
                    // 대상 사용자가 온라인이 아닌 경우 적절한 메시지를 표시합니다.
                    string offlineMessage = $"<color=red>{targetUser}님은 온라인이 아닙니다.</color>";
                    chatMessages.Add(offlineMessage);
                    UpdateChatDisplay();
                    return;
                }

                // 개인 메시지를 보냅니다.
                chatClient.SendPrivateMessage(targetUser, message, false);

                // 보낸 메시지를 채팅 디스플레이에 표시합니다.
                string sentWhisperMessage = $"<color=blue>[귓속말] {targetUser} << {message}</color>";
                chatMessages.Add(sentWhisperMessage); // 보낸 귓속말 메시지를 리스트에 추가
                UpdateChatDisplay(); // 채팅 디스플레이 업데이트

            }
        }

        private void OnInputEndEdit(string input)
        {
            // 명령어 파싱
            if (input.StartsWith("/"))
            {
                if (input.StartsWith("/귓속말 ") || input.StartsWith("/귓말 "))
                {
                    string[] tokens = input.Split(' ');
                    if (tokens.Length >= 3)
                    {
                        // 귓속말 명령어 처리
                        HandleWhisperCommand(tokens);
                        chatinput.text = ""; // 입력 필드 초기화
                        return;
                    }

                    else
                    {
                        Debug.LogWarning("귓속말 명령어 형식이 올바르지 않습니다. /귓속말 대상사용자이름 메시지 or /귓말 대상사용자이름 메시지");
                        // 보낸 메시지를 채팅 디스플레이에 표시합니다.
                        string warning = $"<color=red>명령어가 올바르지 않습니다. /귓속말 대상사용자이름 메시지 or /귓말 대상사용자이름 메시지</color>";
                        chatMessages.Add(warning); // 보낸 귓속말 메시지를 리스트에 추가
                        UpdateChatDisplay(); // 채팅 디스플레이 업데이트
                    }
                }

                else
                {
                    Debug.LogWarning("귓속말 명령어 형식이 올바르지 않습니다. /귓속말 대상사용자이름 메시지 or /귓말 대상사용자이름 메시지");
                    string warning = $"<color=red>명령어가 올바르지 않습니다. /귓속말 대상사용자이름 메시지 or /귓말 대상사용자이름 메시지</color>";
                    chatMessages.Add(warning); // 보낸 귓속말 메시지를 리스트에 추가
                    UpdateChatDisplay(); // 채팅 디스플레이 업데이트
                }
            }

            // 여기서 다른 명령어들을 처리할 수 있습니다.

            // 디폴트로는 채팅 메시지로 처리
            SendMessage();
            chatinput.text = ""; // 입력 필드 초기화
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
            if (!string.IsNullOrEmpty(chatinput.text) && !chatinput.text.StartsWith("/"))
            {
                chatClient.PublishMessage("global", chatinput.text); // 글로벌 채널에 메시지 전송
                chatinput.text = ""; // 인풋 필드 초기화
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

            chatDisplay.text = string.Join("\n", chatMessages); // 채팅 메시지를 텍스트로 변환하여 표시

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f; // 스크롤을 가장 아래로 설정
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
