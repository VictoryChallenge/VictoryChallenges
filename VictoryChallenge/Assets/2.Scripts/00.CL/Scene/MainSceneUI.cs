using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.CL
{ 
    public class MainMenuUI : UI_Scene
    {
        enum Buttons
        {
            JoinRoomButton,
            CustomizeButton,
            ShopButton,
            SettingsButton,
            ExitGameButton,
        }

        private void Start()
        {
            Init();
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));

            GetButton((int)Buttons.JoinRoomButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            //GetButton((int)Buttons.CustomizeButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
            //GetButton((int)Buttons.ShopButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 3));
            //GetButton((int)Buttons.SettingsButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 4));
            //GetButton((int)Buttons.ExitGameButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 5));
        }

        public void OnButtonClicked(PointerEventData data, int a)
        {
            switch (a)
            {
                case 1:
                    Managers.UI.ShowPopupUI<JoinRoomPopup>();
                    break;
                case 2:
                    // Ä¿½ºÅÒ ¾À ·Îµå
                    break;
                case 3:
                    // »óÁ¡ ¾À ¶ç¿ì±â
                    break;
                case 4:
                    Managers.UI.ShowPopupUI<SettingsPopup>();
                    break;
                case 5:
                    // °ÔÀÓ³ª°¡±â
                    // Application.Quit();
                    break;
                default:
                    Debug.LogWarning("Unhandled action: " + a);
                    break;
            }
        }

    }
}
