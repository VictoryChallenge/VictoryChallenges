using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VictoryChallenge.KJ.Photon;
using VictoryChallenge.KJ.Room;
using VictoryChallenge.KJ.Lobby;
using System;
using GSpawn;

namespace VictoryChallenge.Scripts.CL
{ 
    public class LobbySceneUI : UI_Scene
    {
        enum Buttons
        { 
            StageSelectButton,
            GameStart,
            LeaveLobby,
        }

        enum TMPs
        { 
            StageName,
            ReadyOrStart,
        }

        enum Images
        {
            PlayerListContent,
        }

        private Image stageSelectButtonImage;
        private List<int> stageNum = new List<int> { 6 };

        void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            Managers.UI.ShowSceneUI<UI_Scene>("ChatPrefabs");
            Init();
        }

        private int GetRandomStage()
        {
            int randomIndex = UnityEngine.Random.Range(0, stageNum.Count); // 0���� ����Ʈ�� ũ����� ������ �ε����� ����
            return stageNum[randomIndex]; // ���� �ε����� �ش��ϴ� ���� ��ȯ
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<Image>(typeof(Images));
            Bind<TextMeshProUGUI>(typeof(TMPs));


            PhotonSub.Instance.stageNum = GetRandomStage();

            PhotonSub.Instance._text = GetTextMeshPro((int)TMPs.ReadyOrStart);
            PhotonSub.Instance._button = GetButton((int)Buttons.GameStart);
            PhotonSub.Instance.UpdateButtonText();
            GetButton((int)Buttons.GameStart).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            GetButton((int)Buttons.LeaveLobby).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));

            //if (PlayerList.Instance == null)
            //{
            //    Debug.LogError("PlayerList.Instance is null");
            //}
            //else
            //{
            //    var playerListContentImage = GetImage((int)Images.PlayerListContent);
            //    if (playerListContentImage == null)
            //    {
            //        Debug.LogError("playerListContentImage is null");
            //    }
            //    else
            //    {
            //        PlayerList.Instance.playerListContent = playerListContentImage.gameObject;
            //        Debug.Log(playerListContentImage.name);
            //    }
            //}
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // ä�� �Է� �ʵ尡 ��Ŀ���� ��� ó��
                GameObject chatInput = GameObject.Find("ChatInput");
                if (chatInput != null && chatInput.GetComponent<TMP_InputField>().isFocused)
                {
                    chatInput.GetComponent<TMP_InputField>().DeactivateInputField();
                    return; // �� �̻��� ó�� ����
                }

                // �˾� ������ ������� �ʴٸ� �ֻ��� �˾� �ݱ�
                if (Managers.UI.IsPopupStackEmpty() == false)
                {
                    Managers.UI.ClosePopupUI();
                }
                else
                {
                    // �˾� ������ ��� �ִٸ� GameSettingPopup�� ǥ��
                    Managers.UI.ShowPopupUI<GameSettingPopup>();
                }
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (!Managers.UI.IsPopupUIExists<KeyGuidePopup>())
                {
                    Managers.UI.ShowPopupUI<KeyGuidePopup>();
                }
            }
        }

        public void OnButtonClicked(PointerEventData data, int a)
        {
            Managers.Sound.Play("Click", Define.Sound.Effect);

            switch (a)
            {
                case 1:
                    Debug.Log("1");
                    break;
                case 2:
                    Debug.Log("3");
                    LeftLobby();
                    break;
                default:
                    Debug.LogWarning("Unhandled action: " + a);
                    break;
            }
        }

        public void LeftLobby()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
