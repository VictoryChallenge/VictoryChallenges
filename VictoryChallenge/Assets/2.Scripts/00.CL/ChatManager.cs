using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private bool inputbool;

        void Start()
        {
            Init();
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;

            PhotonNetwork.IsMessageQueueRunning = true; // 메시지 큐 실행

            InitializeChat(PhotonNetwork.NickName); // 채팅 초기화
            //InitializeChat("말랑이");
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

            if (!chatinput.isFocused)
            { 
                Controllers.Player.CharacterController[] cc = GameObject.FindObjectsOfType<Controllers.Player.CharacterController>();
                foreach (Controllers.Player.CharacterController c in cc)
                {
                    if (c.isKeyActive == false)
                    { 
                        c.isKeyActive = true;
                        Debug.Log($"{c.isKeyActive} + 트루");
                    }
                }
            }

            // 인풋 필드가 비어 있지 않고 Enter 키를 누르면 메시지 전송
            if (!string.IsNullOrEmpty(chatinput.text) && Input.GetKeyDown(KeyCode.Return))
            {
                inputbool = false;
                OnInputEndEdit(chatinput.text);
                Debug.Log("1번비활성");
                chatinput.DeactivateInputField();
                chatinput.OnDeselect(null);
                chatinput.text = string.Empty;
                ClickCenterOfScreen();

                return;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                inputbool = true;
            }

            // 인풋 필드가 포커스를 받지 않은 상태에서 Enter 키를 누르면 인풋 필드 활성화 및 선택
            if (!chatinput.isFocused && Input.GetKeyDown(KeyCode.Return) && inputbool == true)
            {
                Debug.Log("1번활성");
                chatinput.ActivateInputField();
                chatinput.Select();
                
                Controllers.Player.CharacterController[] cc = GameObject.FindObjectsOfType<Controllers.Player.CharacterController>();
                foreach (Controllers.Player.CharacterController c in cc)
                {
                    c.isKeyActive = false;
                    Debug.Log($"{c.isKeyActive} + 펄스");
                }
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
                    //UpdateChatDisplay();
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

            SendMessage();
            chatinput.DeactivateInputField();
            Debug.Log("edit비활성화리스너");
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
            if (chatinput.text.Trim() == "")
            {
                chatinput.DeactivateInputField();
                chatinput.OnDeselect(null);
                return;
            }

            if (!string.IsNullOrEmpty(chatinput.text) && !chatinput.text.StartsWith("/"))
            {
                chatClient.PublishMessage("global", chatinput.text); // 글로벌 채널에 메시지 전송
                chatinput.text = ""; // 입력 필드 초기화
                Debug.Log("센드");
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

        void ClickCenterOfScreen()
        {
            // 화면 가운데 위치 계산
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

            // 새로운 PointerEventData 객체 생성
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = screenCenter
            };

            // Raycast 결과를 저장할 리스트 생성
            List<RaycastResult> results = new List<RaycastResult>();

            // Raycast를 통해 UI 요소 감지
            EventSystem.current.RaycastAll(pointerData, results);

            // 감지된 UI 요소들에 대해 클릭 이벤트 발생
            foreach (RaycastResult result in results)
            {
                ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
                Debug.Log("Clicked on UI element: " + result.gameObject.name);
            }
        }
    }
}
