using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VictoryChallenge.KJ.Auth;

namespace VictoryChallenge.Scripts.CL
{
    public class RegisterPopup : UI_Popup
    {
        enum InputFields
        { 
            Email,
            Password,
            PasswordCheck,
            Nickname,
        }

        enum Buttons
        { 
            Register,
        }

        enum TMPs
        { 
            ErrorText,
            ConfirmText,
        }

        void Start()
        {
            Init();
        }

        TMP_InputField _email;
        TMP_InputField _password;
        TMP_InputField _passwordCheck;
        TMP_InputField _nickname;


        public override void Init()
        {
            base.Init();
            Bind<TMP_InputField>(typeof(InputFields));
            Bind<Button>(typeof(Buttons));
            Bind<TextMeshProUGUI>(typeof(TMPs));

            _email = GetInputField((int)InputFields.Email);
            _password = GetInputField((int) InputFields.Password);
            _passwordCheck = GetInputField((int)InputFields.PasswordCheck);
            _nickname = GetInputField((int)InputFields.Nickname);

            GetButton((int)Buttons.Register).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            Authentication.Instance.confirmRegisterText = GetTextMeshPro((int)TMPs.ConfirmText);
            Authentication.Instance.warningRegisterText = GetTextMeshPro((int)TMPs.ErrorText);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePopupUI();
            }
        }

        //public void NavigateThroughInputField(List<TMP_InputField> inputFields)
        //{
        //    for (int i = 0; i < inputFields.Count; i++)
        //    {
        //        if (inputFields[i].isFocused)
        //        {
        //            int nextIndex = (i + 1) % inputFields.Count;
        //            inputFields[nextIndex].Select();
        //            inputFields[nextIndex].ActivateInputField();
        //            break;
        //        }
        //    }
        //}

        void OnButtonClicked(PointerEventData data, int a)
        {
            switch (a)
            { 
                case 1:
                    if (_email != null && _password != null && _passwordCheck != null && _nickname != null)
                    {
                        Authentication.Instance.emailRegister = _email;
                        Authentication.Instance.passwordRegister = _password;
                        Authentication.Instance.passwordCheck = _passwordCheck;
                        Authentication.Instance.usernameRegister = _nickname;
                        Authentication.Instance.RegisterButton();
                    }
                    else
                    {
                        Debug.LogError("필드가 초기화되지 않았습니다.");
                    }
                    break;
                default:
                    Debug.LogWarning("Unhandled action: " + a);
                    break;
            }
        }
    }
}
