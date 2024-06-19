using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VictoryChallenge.KJ.Photon;

namespace VictoryChallenge.Scripts.CL
{ 
    public class StageSelectPopup : UI_Popup
    {
        enum Buttons
        { 
            Justrun1,
            Justrun2,
            Marathon,
            OnlyOne,
        }

        Sprite _justrunSp;
        Sprite _justrun2Sp;
        Sprite _marathonSp;
        Sprite _onlyoneSp;
        string _justrun1 = "��ֹ� �޸���";
        string _justrun2 = "��ֹ� �޸���2";
        string _marathon = "���� ������";
        string _onlyone = "onlyone";

        public Action<Sprite, string> OnStageSelected;

        void Start()
        {
            Init();
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public override void Init()
        {
            base.Init();

            Bind<Button>(typeof(Buttons));

            _justrunSp = GetButton((int)Buttons.Justrun1).gameObject.GetComponent<Image>().sprite;
            _justrun2Sp = GetButton((int)Buttons.Justrun2).gameObject.GetComponent<Image>().sprite;
            _marathonSp = GetButton((int)Buttons.Marathon).gameObject.GetComponent<Image>().sprite;
            _onlyoneSp = GetButton((int)Buttons.OnlyOne).gameObject.GetComponent<Image>().sprite;

            GetButton((int)Buttons.Justrun1).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            GetButton((int)Buttons.Justrun2).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
            GetButton((int)Buttons.Marathon).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 3));
            GetButton((int)Buttons.OnlyOne).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 4));
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePopupUI();
            }
        }

        public void OnButtonClicked(PointerEventData data, int a)
        {
            Managers.Sound.Play("Click", Define.Sound.Effect);

            Sprite selectedSprite = null;
            string _mapname = null;
            int stageNum = 3;

            switch (a)
            {
                case 1:
                    Debug.Log("����Ʈ��");
                    selectedSprite = _justrunSp;
                    _mapname = _justrun1;
                    stageNum = 3;
                    break;
                case 2:
                    Debug.Log("����Ʈ��2");
                    selectedSprite = _justrun2Sp;
                    _mapname = _justrun2;
                    stageNum = 5; // ����� �������� ��ȣ
                    break;
                case 3:
                    Debug.Log("������");
                    selectedSprite = _marathonSp;
                    _mapname = _marathon;
                    stageNum = 6;
                    break;
                case 4:
                    Debug.Log("OnlyOne");
                    selectedSprite = _onlyoneSp;
                    _mapname = _onlyone;
                    stageNum = 7;
                    break;
                default:
                    Debug.LogWarning("Unhandled action: " + a);
                    break;
            }

            OnStageSelected?.Invoke(selectedSprite, _mapname); // Delegate�� Sprite, string �ѱ��
            PhotonSub.Instance.SetStageNum(stageNum);
            ClosePopupUI();

        }
    }
}
