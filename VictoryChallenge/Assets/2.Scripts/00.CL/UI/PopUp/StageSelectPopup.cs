using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.CL
{ 
    public class StageSelectPopup : UI_Popup
    {
        enum Buttons
        { 
            Justrun1,
            OnlyOne,
            Marathon,
            OnlyUp,
        }

        Sprite _justrunSp;
        Sprite _onlyoneSp;
        Sprite _marathonSp;
        Sprite _onlyupSp;
        string _justrun1 = "장애물 달리기";
        string _onlyone = "onlyone";
        string _marathon = "혈압 마라톤";
        string _onlyup = "onlyup";

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
            _onlyoneSp = GetButton((int)Buttons.OnlyOne).gameObject.GetComponent<Image>().sprite;
            _marathonSp = GetButton((int)Buttons.Marathon).gameObject.GetComponent<Image>().sprite;
            _onlyupSp = GetButton((int)Buttons.OnlyUp).gameObject.GetComponent<Image>().sprite;

            GetButton((int)Buttons.Justrun1).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            GetButton((int)Buttons.OnlyOne).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
            GetButton((int)Buttons.Marathon).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 3));
            GetButton((int)Buttons.OnlyUp).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 4));
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
            Sprite selectedSprite = null;
            string _mapname = null;

            switch (a)
            {
                case 1:
                    Debug.Log("저스트런");
                    selectedSprite = _justrunSp;
                    _mapname = _justrun1;
                    break;
                case 2:
                    Debug.Log("OnlyOne");
                    selectedSprite = _onlyoneSp;
                    _mapname = _onlyone;
                    break;
                case 3:
                    Debug.Log("마라톤");
                    selectedSprite = _marathonSp;
                    _mapname = _marathon;
                    break;
                case 4:
                    Debug.Log("OnlyUp");
                    selectedSprite = _onlyupSp;
                    _mapname = _onlyup;
                    break;
                default:
                    Debug.LogWarning("Unhandled action: " + a);
                    break;
            }

            OnStageSelected?.Invoke(selectedSprite, _mapname); // Delegate로 Sprite, string 넘기기
            ClosePopupUI();
        }
    }
}
