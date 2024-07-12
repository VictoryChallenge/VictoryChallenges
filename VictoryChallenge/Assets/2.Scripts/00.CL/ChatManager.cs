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
    /// ä�� ����� �����ϸ�, �޽��� �ۼ���, ä�� �α� ǥ��, ���� �޽��� ó���� �����ϴ� Ŭ����
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
        TextMeshProUGUI chatDisplay; // ä�� �޽��� ǥ�� �ؽ�Ʈ
        ScrollRect scrollRect; // ��ũ�� ��

        ChatClient chatClient; // ä�� Ŭ���̾�Ʈ
        string lastMessages; // ������ �޽���
        string roomName; // �� �̸�

        private List<string> chatMessages = new List<string>(); // ä�� �޽��� ����Ʈ
        private string userName; // ����� �̸�
        private bool inputbool;

        void Start()
        {
            Init();
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;

            PhotonNetwork.IsMessageQueueRunning = true; // �޽��� ť ����

            InitializeChat(PhotonNetwork.NickName); // ä�� �ʱ�ȭ
            //InitializeChat("������");
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
                chatClient.Service(); // ä�� Ŭ���̾�Ʈ ���� ����
            }

            if (!chatinput.isFocused)
            { 
                Controllers.Player.CharacterController[] cc = GameObject.FindObjectsOfType<Controllers.Player.CharacterController>();
                foreach (Controllers.Player.CharacterController c in cc)
                {
                    if (c.isKeyActive == false)
                    { 
                        c.isKeyActive = true;
                        Debug.Log($"{c.isKeyActive} + Ʈ��");
                    }
                }
            }

            // ��ǲ �ʵ尡 ��� ���� �ʰ� Enter Ű�� ������ �޽��� ����
            if (!string.IsNullOrEmpty(chatinput.text) && Input.GetKeyDown(KeyCode.Return))
            {
                inputbool = false;
                OnInputEndEdit(chatinput.text);
                Debug.Log("1����Ȱ��");
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

            // ��ǲ �ʵ尡 ��Ŀ���� ���� ���� ���¿��� Enter Ű�� ������ ��ǲ �ʵ� Ȱ��ȭ �� ����
            if (!chatinput.isFocused && Input.GetKeyDown(KeyCode.Return) && inputbool == true)
            {
                Debug.Log("1��Ȱ��");
                chatinput.ActivateInputField();
                chatinput.Select();
                
                Controllers.Player.CharacterController[] cc = GameObject.FindObjectsOfType<Controllers.Player.CharacterController>();
                foreach (Controllers.Player.CharacterController c in cc)
                {
                    c.isKeyActive = false;
                    Debug.Log($"{c.isKeyActive} + �޽�");
                }
            }
        }


        /// <summary>
        /// ä���� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="playerName">�÷��̾� �̸�</param>
        public void InitializeChat(string playerName)
        {
            userName = playerName; // ����� �̸� ����
            chatClient = new ChatClient(this); // ä�� Ŭ���̾�Ʈ �ʱ�ȭ
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(userName)); // ä�� ������ ����
        }

        /// <summary>
        /// ä�� ���� ����� �޽����� ����մϴ�.
        /// </summary>
        public void DebugReturn(DebugLevel level, string message)
        {
            // ���� ����
        }

        /// <summary>
        /// ä�� ���� ���� �� ȣ��˴ϴ�.
        /// </summary>
        public void OnChatStateChange(ChatState state)
        {
            Debug.Log("Chat state changed to : " + state); // ���ο� ä�� ���¸� ���
        }

        /// <summary>
        /// ä�� ������ ����Ǿ��� �� ȣ��˴ϴ�.
        /// </summary>
        public void OnConnected()
        {
            chatClient.Subscribe(new string[] { "global" }); // �۷ι� ä�ο� ����, ���÷ε� ���� ����
            Debug.Log("Connected to chat"); // ���� ���� �޽��� ���
        }

        /// <summary>
        /// ä�� �������� ������ ������ �� ȣ��˴ϴ�.
        /// </summary>
        public void OnDisconnected()
        {
            Debug.Log("Disconnected from chat"); // ���� ���� �޽��� ���
        }

        /// <summary>
        /// ä�� �޽����� �������� �� ȣ��˴ϴ�.
        /// </summary>
        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            for (int i = 0; i < messages.Length; i++)
            {
                if (channelName == "global" && !messages[i].ToString().StartsWith("/"))
                {
                    chatMessages.Add($"{senders[i]} : {messages[i]}"); // �޽����� ����Ʈ�� �߰�
                    lastMessages = messages[i].ToString(); // ������ �޽��� ������Ʈ
                }
            }
            UpdateChatDisplay(); // ä�� ���÷��� ������Ʈ
        }

        /// <summary>
        /// ���� �޽����� �������� �� ȣ��˴ϴ�.
        /// </summary>
        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            string senderId = null;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.NickName == sender)
                {
                    senderId = player.UserId; // ��� ������� UID�� ����
                    break;
                }
            }

            if (sender != PhotonNetwork.NickName || senderId != PhotonNetwork.LocalPlayer.UserId)
            {
                string whisperMessage = $"<color=blue>[�ӼӸ�] {sender} >> {message}</color>";
                chatMessages.Add(whisperMessage);
                UpdateChatDisplay();
            }
        }

        /// <summary>
        /// �ӼӸ� ��ɾ ó���մϴ�.
        /// </summary>
        /// <param name="tokens">��ɾ� ��ū</param>
        public void HandleWhisperCommand(string[] tokens)
        {
            // ��ɾ �м��Ͽ� ��� ����� �̸��� ���� �޽����� �����մϴ�.
            if (tokens.Length >= 3)
            {
                string targetUser = tokens[1]; // ��� ����� �̸�
                string message = string.Join(" ", tokens, 2, tokens.Length - 2); // ���� �޽���

                // PhotonNetwork.PlayerList�� ����Ͽ� ��� ������� UID�� ã���ϴ�.
                string targetUserId = null;
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (player.NickName == targetUser)
                    {
                        targetUserId = player.UserId; // ��� ������� UID�� ����
                        break;
                    }
                }

                if (targetUser == PhotonNetwork.NickName || targetUser == PhotonNetwork.LocalPlayer.UserId)
                {
                    string sendMeWarning = $"<color=red>�ڽſ��Դ� �޽����� ���� �� �����ϴ�.</color>";
                    chatMessages.Add(sendMeWarning); // ��� �޽����� ����Ʈ�� �߰�
                    UpdateChatDisplay(); // ä�� ���÷��� ������Ʈ
                    return;
                }

                // ��� ����ڰ� �¶������� Ȯ��
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
                    // ��� ����ڰ� �¶����� �ƴ� ��� ������ �޽����� ǥ���մϴ�.
                    string offlineMessage = $"<color=red>{targetUser}���� �¶����� �ƴմϴ�.</color>";
                    chatMessages.Add(offlineMessage);
                    //UpdateChatDisplay();
                    return;
                }

                // ���� �޽����� �����ϴ�.
                chatClient.SendPrivateMessage(targetUser, message, false);

                // ���� �޽����� ä�� ���÷��̿� ǥ���մϴ�.
                string sentWhisperMessage = $"<color=blue>[�ӼӸ�] {targetUser} << {message}</color>";
                chatMessages.Add(sentWhisperMessage); // ���� �ӼӸ� �޽����� ����Ʈ�� �߰�
                UpdateChatDisplay(); // ä�� ���÷��� ������Ʈ

            }
        }

        private void OnInputEndEdit(string input)
        {
            // ��ɾ� �Ľ�
            if (input.StartsWith("/"))
            {
                if (input.StartsWith("/�ӼӸ� ") || input.StartsWith("/�Ӹ� "))
                {
                    string[] tokens = input.Split(' ');
                    if (tokens.Length >= 3)
                    {
                        // �ӼӸ� ��ɾ� ó��
                        HandleWhisperCommand(tokens);
                        chatinput.text = ""; // �Է� �ʵ� �ʱ�ȭ
                        return;
                    }

                    else
                    {
                        Debug.LogWarning("�ӼӸ� ��ɾ� ������ �ùٸ��� �ʽ��ϴ�. /�ӼӸ� ��������̸� �޽��� or /�Ӹ� ��������̸� �޽���");
                        // ���� �޽����� ä�� ���÷��̿� ǥ���մϴ�.
                        string warning = $"<color=red>��ɾ �ùٸ��� �ʽ��ϴ�. /�ӼӸ� ��������̸� �޽��� or /�Ӹ� ��������̸� �޽���</color>";
                        chatMessages.Add(warning); // ���� �ӼӸ� �޽����� ����Ʈ�� �߰�
                        UpdateChatDisplay(); // ä�� ���÷��� ������Ʈ
                    }
                }

                else
                {
                    Debug.LogWarning("�ӼӸ� ��ɾ� ������ �ùٸ��� �ʽ��ϴ�. /�ӼӸ� ��������̸� �޽��� or /�Ӹ� ��������̸� �޽���");
                    string warning = $"<color=red>��ɾ �ùٸ��� �ʽ��ϴ�. /�ӼӸ� ��������̸� �޽��� or /�Ӹ� ��������̸� �޽���</color>";
                    chatMessages.Add(warning); // ���� �ӼӸ� �޽����� ����Ʈ�� �߰�
                    UpdateChatDisplay(); // ä�� ���÷��� ������Ʈ
                }
            }

            SendMessage();
            chatinput.DeactivateInputField();
            Debug.Log("edit��Ȱ��ȭ������");
        }

        /// <summary>
        /// ����� ���°� ������Ʈ�Ǿ��� �� ȣ��˴ϴ�.
        /// </summary>
        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            // ���� ����
        }

        /// <summary>
        /// ä�ο� �������� �� ȣ��˴ϴ�.
        /// </summary>
        public void OnSubscribed(string[] channels, bool[] results)
        {
            Debug.Log("Subscribed to channels: " + string.Join(", ", channels)); // ������ ä�ε��� ���
        }

        /// <summary>
        /// ä�ο��� Ż������ �� ȣ��˴ϴ�.
        /// </summary>
        public void OnUnsubscribed(string[] channels)
        {
            // ���� ����
        }

        /// <summary>
        /// ����ڰ� ä�ο� �������� �� ȣ��˴ϴ�.
        /// </summary>
        public void OnUserSubscribed(string channel, string user)
        {
            // ���� ����
        }

        /// <summary>
        /// ����ڰ� ä�ο��� Ż������ �� ȣ��˴ϴ�.
        /// </summary>
        public void OnUserUnsubscribed(string channel, string user)
        {
            // ���� ����
        }

        /// <summary>
        /// �޽����� �����մϴ�.
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
                chatClient.PublishMessage("global", chatinput.text); // �۷ι� ä�ο� �޽��� ����
                chatinput.text = ""; // �Է� �ʵ� �ʱ�ȭ
                Debug.Log("����");
            }
        }

        /// <summary>
        /// ä�� ���÷��̸� ������Ʈ�մϴ�.
        /// </summary>
        private void UpdateChatDisplay()
        {
            if (chatMessages.Count > 30)
            {
                chatMessages.RemoveRange(0, chatMessages.Count - 30); // �޽����� 30���� �ʰ��ϸ� ������ �޽��� ����
                Debug.Log("dd");
            }

            chatDisplay.text = string.Join("\n", chatMessages); // ä�� �޽����� �ؽ�Ʈ�� ��ȯ�Ͽ� ǥ��

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f; // ��ũ���� ���� �Ʒ��� ����
        }

        void ClickCenterOfScreen()
        {
            // ȭ�� ��� ��ġ ���
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

            // ���ο� PointerEventData ��ü ����
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = screenCenter
            };

            // Raycast ����� ������ ����Ʈ ����
            List<RaycastResult> results = new List<RaycastResult>();

            // Raycast�� ���� UI ��� ����
            EventSystem.current.RaycastAll(pointerData, results);

            // ������ UI ��ҵ鿡 ���� Ŭ�� �̺�Ʈ �߻�
            foreach (RaycastResult result in results)
            {
                ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
                Debug.Log("Clicked on UI element: " + result.gameObject.name);
            }
        }
    }
}
