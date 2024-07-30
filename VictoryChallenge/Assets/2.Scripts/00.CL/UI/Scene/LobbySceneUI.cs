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
            int randomIndex = UnityEngine.Random.Range(0, stageNum.Count); // 0부터 리스트의 크기까지 랜덤한 인덱스를 선택
            return stageNum[randomIndex]; // 랜덤 인덱스에 해당하는 값을 반환
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
                GameObject go = GameObject.Find("GameSettingPopup");
                if (go != null)
                    return;
                else
                    Managers.UI.ShowPopupUI<GameSettingPopup>();

                GameObject gogo = GameObject.Find("ChatInput");
                if (gogo != null && gogo.GetComponent<TMP_InputField>().isFocused)
                    gogo.GetComponent<TMP_InputField>().DeactivateInputField();
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
