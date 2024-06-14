using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
            AuthenticationTest.Instance.confirmLoginText = GetTextMeshPro((int)TMPs.ConfirmText);
            AuthenticationTest.Instance.warningLoginText = GetTextMeshPro((int)TMPs.ErrorText);
        }

        public void OnButtonClicked(PointerEventData data, int a)
        {
            switch (a)
            {
                case 1:
                    Debug.Log("Attempting to login...");
                    AuthenticationTest.Instance.email = _email;
                    AuthenticationTest.Instance.password = _password;
                    AuthenticationTest.Instance.AttemptLogin();
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
