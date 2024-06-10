using System.Collections;
using System.Collections.Generic;
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

        enum Images
        { 
            PlayerListContent,
        }

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

            GetButton((int)Buttons.StageSelectButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            GetButton((int)Buttons.GameStart).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
            GetButton((int)Buttons.LeaveLobby).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 3));
        }

        public void OnButtonClicked(PointerEventData data, int a)
        {
            switch (a)
            {
                case 1:
                    Debug.Log("1");
                    // stageselect popup 띄우기
                    break;
                case 2:
                    Debug.Log("2");
                    // PhotonNetwork.LoadLevel로 GameStart 넘기기
                    break;
                case 3:
                    Debug.Log("3");
                    // Leave Lobby 추가 (로비떠나는기능)
                    break;
                default:
                    Debug.LogWarning("Unhandled action: " + a);
                    break;
            }
        }
    }
}
