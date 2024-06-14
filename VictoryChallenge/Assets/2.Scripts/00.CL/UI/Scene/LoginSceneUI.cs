using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VictoryChallenge.KJ.Auth;
using VictoryChallenge.KJ.Photon;

namespace VictoryChallenge.Scripts.CL
{ 
    public class LoginSceneUI : UI_Scene
    {
        enum InputFields
        { 
            Email,
            Password,
        }

        enum Buttons
        { 
            StartGame,
            Register,
        }

        enum TMPs
        { 
            ErrorText,
            ConfirmText,
        }

        private TMP_InputField _email;
        private TMP_InputField _password;

        void Start()
        {
            Init();
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<TMP_InputField>(typeof(InputFields));
            Bind<TextMeshProUGUI>(typeof(TMPs));

            _email = GetInputField((int)InputFields.Email);
            _password = GetInputField((int)InputFields.Password);

            GetButton((int)Buttons.StartGame).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            GetButton((int)Buttons.Register).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
            Authentication.Instance.confirmLoginText = GetTextMeshPro((int)TMPs.ConfirmText);
            Authentication.Instance.warningLoginText = GetTextMeshPro((int)TMPs.ErrorText);
        }

        public void OnButtonClicked(PointerEventData data, int a)
        {
            switch (a)
            {
                case 1:
                    Debug.Log("Attempting to login...");
                    Authentication.Instance.email = _email;
                    Authentication.Instance.password = _password;
                    Authentication.Instance.AttemptLogin();
                    break;
                case 2:
                    Managers.UI.ShowPopupUI<RegisterPopup>("RegisterPopup");
                    break;
                default:
                    Debug.LogWarning("Unhandled action: " + a);
                    break;
            }
        }
    }
}
