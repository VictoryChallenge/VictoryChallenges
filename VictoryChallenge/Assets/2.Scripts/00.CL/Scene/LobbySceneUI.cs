using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        enum ScrollViews
        { 
            ChatScrollView,
        }

        enum InputFields
        { 
            ChatInput,
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
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<ScrollRect>(typeof(ScrollViews));
            Bind<TMP_InputField>(typeof(InputFields));
            Bind<Image>(typeof(Images));
            Bind<TextMeshProUGUI>(typeof(TMPs));

            stageSelectButtonImage = GetButton((int)Buttons.StageSelectButton).GetComponent<Image>();

            if (PhotonNetwork.IsMasterClient)
            {
                GetTextMeshPro((int)TMPs.ReadyOrStart).text = "Start";
                GetButton((int)Buttons.GameStart).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            }
            else GetTextMeshPro((int)TMPs.ReadyOrStart).text = "Ready";

            GetButton((int)Buttons.StageSelectButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
            GetButton((int)Buttons.LeaveLobby).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 3));
        }

        public void OnButtonClicked(PointerEventData data, int a)
        {
            switch (a)
            {
                case 1:
                    Debug.Log("1");
                    // PhotonNetwork.LoadLevel�� GameStart �ѱ��
                    break;
                case 2:
                    var stageSelectPopup = Managers.UI.ShowPopupUI<StageSelectPopup>();
                    stageSelectPopup.OnStageSelected += UpdateStageSelectTextSprite; // �̺�Ʈ ����
                    break;
                case 3:
                    Debug.Log("3");
                    // Leave Lobby �߰� (�κ񶰳��±��)
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
    }
}
