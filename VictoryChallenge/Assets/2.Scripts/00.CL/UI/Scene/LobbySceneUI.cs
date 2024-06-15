using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VictoryChallenge.KJ.Photon;

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

        void Start()
        {
            Init();
            Managers.UI.ShowSceneUI<UI_Scene>("ChatPrefabs");
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<Image>(typeof(Images));
            Bind<TextMeshProUGUI>(typeof(TMPs));

            stageSelectButtonImage = GetButton((int)Buttons.StageSelectButton).GetComponent<Image>();

            PhotonSub.Instance._text = GetTextMeshPro((int)TMPs.ReadyOrStart);
            PhotonSub.Instance._button = GetButton((int)Buttons.GameStart);
            PhotonSub.Instance.UpdateButtonText();
            GetButton((int)Buttons.GameStart).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));

            GetButton((int)Buttons.StageSelectButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
            GetButton((int)Buttons.LeaveLobby).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 3));
        }

        public void OnButtonClicked(PointerEventData data, int a)
        {
            switch (a)
            {
                case 1:
                    Debug.Log("1");
                    // 게임시작 혹은 레디 ㅋㅋ
                    break;
                case 2:
                    var stageSelectPopup = Managers.UI.ShowPopupUI<StageSelectPopup>();
                    stageSelectPopup.OnStageSelected += UpdateStageSelectTextSprite; // 이벤트 구독
                    break;
                case 3:
                    Debug.Log("3");
                    LeaveLobby();
                    break;
                default:
                    Debug.LogWarning("Unhandled action: " + a);
                    break;
            }
        }

        private void UpdateStageSelectTextSprite(Sprite newSprite, string name)
        {
            if (newSprite != null)
                stageSelectButtonImage.sprite = newSprite;
            if (name != null)
                GetTextMeshPro((int)TMPs.StageName).text = name;
        }

        private void LeaveLobby()
        {
            PhotonNetwork.LeaveRoom(); // 포톤 네트워크에서 현재 방을 떠남
            SceneManager.LoadScene(1); // 로비 씬 다시 로드
        }
    }
}
