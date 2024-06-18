using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VictoryChallenge.KJ.Photon;

namespace VictoryChallenge.Scripts.CL
{ 
    public class JoinRoomPopup : UI_Popup
    {
        enum Buttons
        {
            SearchingGameButton,
            exitButton,
        }

        enum InputFields
        {
            NickNameInput,
        }

        private void Start()
        {
            Init();
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            // ���ֱ�
            Bind<TMP_InputField>(typeof(InputFields));

            GetButton((int)Buttons.SearchingGameButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            GetButton((int)Buttons.exitButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePopupUI();
            }
        }

        public void OnButtonClicked(PointerEventData data, int a)
        {
            switch (a)
            {
                case 1:
                    // ���߿����ֱ�.�׽�Ʈ��
                    // PhotonNetwork.NickName = GetInputField((int)InputFields.NickNameInput).text;
                    PhotonManager.Instance.QuickMatch();
                    break;
                case 2:
                    // �˾� ����
                    ClosePopupUI();
                    break;
                default:
                    Debug.LogWarning("Unhandled action: " + a);
                    break;
            }
        }
    }
}
