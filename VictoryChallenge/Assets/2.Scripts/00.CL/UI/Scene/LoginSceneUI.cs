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
using static LoginManager;

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
        public List<TMP_InputField> loginInputFields = new List<TMP_InputField>();

        void Start()
        {
            Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                NavigateThroughInputField(loginInputFields);
            }
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<TMP_InputField>(typeof(InputFields));
            Bind<TextMeshProUGUI>(typeof(TMPs));
            Managers.Sound.Play("MainBGM", Define.Sound.BGM);
            AudioSource _audioSource = GameObject.Find("BGM").GetComponent<AudioSource>();
            _audioSource.volume = 0.4f;

            loginInputFields.Add(GetInputField((int)InputFields.Email));
            loginInputFields.Add(GetInputField((int)InputFields.Password));

            _email = GetInputField((int)InputFields.Email);
            _password = GetInputField((int)InputFields.Password);

            GetButton((int)Buttons.StartGame).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            GetButton((int)Buttons.Register).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
            RestAPIAuth.Instance.confirmLoginText = GetTextMeshPro((int)TMPs.ConfirmText);
            RestAPIAuth.Instance.warningLoginText = GetTextMeshPro((int)TMPs.ErrorText);
        }

        public void OnButtonClicked(PointerEventData data, int a)
        {
            Managers.Sound.Play("Click", Define.Sound.Effect);

            switch (a)
            {
                case 1:
                    Debug.Log("Attempting to login...");
                    RestAPIAuth.Instance.email = _email;
                    RestAPIAuth.Instance.password = _password;
                    RestAPIAuth.Instance.AttemptLogin();
                    break;
                case 2:
                    Managers.UI.ShowPopupUI<RegisterPopup>("RegisterPopup");
                    break;
                default:
                    Debug.LogWarning("Unhandled action: " + a);
                    break;
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
    }
}
