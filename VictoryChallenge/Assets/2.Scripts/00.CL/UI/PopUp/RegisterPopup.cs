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
            ExitButton
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
        List<TMP_InputField> registerInputFields = new List<TMP_InputField>();

        public override void Init()
        {
            base.Init();
            Bind<TMP_InputField>(typeof(InputFields));
            Bind<Button>(typeof(Buttons));
            Bind<TextMeshProUGUI>(typeof(TMPs));

            registerInputFields.Add(GetInputField(0));
            registerInputFields.Add(GetInputField(1));
            registerInputFields.Add(GetInputField(2));
            registerInputFields.Add(GetInputField(3));

            _email = GetInputField((int)InputFields.Email);
            _password = GetInputField((int) InputFields.Password);
            _passwordCheck = GetInputField((int)InputFields.PasswordCheck);
            _nickname = GetInputField((int)InputFields.Nickname);

            GetButton((int)Buttons.Register).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            GetButton((int)Buttons.ExitButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
            Authentication.Instance.confirmRegisterText = GetTextMeshPro((int)TMPs.ConfirmText);
            Authentication.Instance.warningRegisterText = GetTextMeshPro((int)TMPs.ErrorText);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePopupUI();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                NavigateThroughInputField(registerInputFields);
            }
        }

        public void NavigateThroughInputField(List<TMP_InputField> inputFields)
        {
            for (int i = 0; i < inputFields.Count; i++)
            {
                if (inputFields[i].isFocused)
                {
                    int nextIndex = (i + 1) % inputFields.Count;
                    inputFields[nextIndex].Select();
                    inputFields[nextIndex].ActivateInputField();
                    break;
                }
            }
        }

        void OnButtonClicked(PointerEventData data, int a)
        {
            Managers.Sound.Play("Click", Define.Sound.Effect);

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
                case 2:
                    ClosePopupUI();
                    break;
                default:
                    Debug.LogWarning("Unhandled action: " + a);
                    break;
            }
        }
    }
}
