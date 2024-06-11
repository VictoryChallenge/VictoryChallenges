using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.CL
{
    public class ChatManager : MonoBehaviour, IChatClientListener
    {
        public TMP_InputField inputField;
        public TextMeshProUGUI chatDisplay;
        public ScrollRect scrollRect;
        public ChatClient chatClient;
        public string lastMessages;
        string roomName;

        private List<string> chatMessages = new List<string>();
        private string userName;

        void Start()
        {
            PhotonNetwork.IsMessageQueueRunning = true;

            InitializeChat(PhotonNetwork.NickName);
        }

        void Update()
        {
            if (chatClient != null)
            {
                chatClient.Service();
            }

            if (!inputField.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                inputField.ActivateInputField();
                inputField.Select();
            }

            if (!string.IsNullOrEmpty(inputField.text) && Input.GetKeyDown(KeyCode.Return))
            {
                SendMessage();
            }
        }

        public void InitializeChat(string playerName)
        {
            userName = playerName;
            chatClient = new ChatClient(this);
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(userName));
        }

        // ä�ð��� ����׸޽��� ����
        public void DebugReturn(DebugLevel level, string message)
        {

        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log("Chat state changed to : " + state);
        }

        public void OnConnected()
        {
            //roomName = PhotonNetwork.CurrentRoom.Name;
            chatClient.Subscribe(new string[] { "global" });
            Debug.Log("Connected to chat");
        }

        public void OnDisconnected()
        {
            Debug.Log("Disconnected from chat");
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            for (int i = 0; i < messages.Length; i++)
            {
                if (channelName == "global")
                {
                    chatMessages.Add($"{senders[i]} : {messages[i]}");
                    lastMessages = messages[i].ToString();
                    //ShowChatBubble(senders[i], messages[i].ToString());
                }
            }
            UpdateChatDisplay();
            //UpdateChatbubble();
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            if (sender != userName)
                chatDisplay.text += $"<color=blue>[�ӼӸ�] {sender} >> {message}</color>\n";
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {

        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            Debug.Log("Subscribed to channels: " + string.Join(", ", channels));
        }

        public void OnUnsubscribed(string[] channels)
        {

        }

        public void OnUserSubscribed(string channel, string user)
        {

        }

        public void OnUserUnsubscribed(string channel, string user)
        {

        }

        public void SendMessage()
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                chatClient.PublishMessage("global", inputField.text);
                inputField.text = "";
            }
        }

        private void UpdateChatDisplay()
        {
            if (chatMessages.Count > 30)
            {
                chatMessages.RemoveRange(0, chatMessages.Count - 30);
                Debug.Log("dd");
            }

            chatDisplay.text = string.Join("\n", chatMessages.ToArray());

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }

        //private void ShowChatBubble(string sender, string message)
        //{
        //    // sender �̸����� ĳ���͸� ã��
        //    GameObject[] characters = GameObject.FindGameObjectsWithTag("Player"); // Assuming the player characters are tagged as "Player"
        //    foreach (var character in characters)
        //    {
        //        // Check if this character belongs to the sender
        //        if (character.name == sender)
        //        {
        //            // ��Ȱ��ȭ�� �ڽ� ������Ʈ�� �����Ͽ� ã�� ���� GetComponentsInChildren ���
        //            ChatBubble chatbubble = character.GetComponentInChildren<ChatBubble>(true);
        //            if (chatbubble != null)
        //            {
        //                chatbubble.gameObject.SetActive(true);
        //                chatbubble.SetText(message); // Assuming ChatBubble has a SetText method to update the message
        //            }
        //            else
        //            {
        //                Debug.LogWarning("ChatBubble component not found in children of " + character.name);
        //            }
        //            break;
        //        }
        //    }
        //}
        //private void ShowChatBubble(string sender, string message)
        //{
        //    // sender �̸����� ĳ���͸� ã��
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

        public void HandleWhisperCommand(string[] tokens)
        {
            // ��ɾ �м��Ͽ� ��� ����� �̸��� ���� �޽����� �����մϴ�.
            if (tokens.Length >= 3)
            {
                string targetUser = tokens[1];
                string message = string.Join(" ", tokens, 2, tokens.Length - 2);

                // ���� �޽����� �����ϴ�.
                chatClient.SendPrivateMessage(targetUser, message, false);

                // ���� �޽����� ä�� ���÷��̿� ǥ���մϴ�.
                chatDisplay.text += $"<color=blue>[�ӼӸ�] {targetUser} << {message}</color>\n";
            }
            else
            {
                Debug.LogWarning("�ӼӸ� ��ɾ� ������ �ùٸ��� �ʽ��ϴ�. /�ӼӸ� ��������̸� �޽��� or /�Ӹ� ��������̸� �޽���");
                chatDisplay.text += "<color=red>��ɾ �ùٸ��� �ʽ��ϴ�. /�ӼӸ� ��������̸� �޽��� or /�Ӹ� ��������̸� �޽���</color>\n";
            }
        }
    }
}
