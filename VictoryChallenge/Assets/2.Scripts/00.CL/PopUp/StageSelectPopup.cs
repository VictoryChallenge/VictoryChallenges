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
            Marathon,
            OnlyOne,
            Zombie,
            OnlyUp,
        }

        Sprite _marathonSp;
        Sprite _onlyoneSp;
        Sprite _zombieSp;
        Sprite _onlyupSp;
        string _marathon = "marathon";
        string _onlyone = "onlyone";
        string _zombie = "zombie";
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

            _marathonSp = GetButton((int)Buttons.Marathon).gameObject.GetComponent<Image>().sprite;
            _onlyoneSp = GetButton((int)Buttons.OnlyOne).gameObject.GetComponent<Image>().sprite;
            _zombieSp = GetButton((int)Buttons.Zombie).gameObject.GetComponent<Image>().sprite;
            _onlyupSp = GetButton((int)Buttons.OnlyUp).gameObject.GetComponent<Image>().sprite;

            GetButton((int)Buttons.Marathon).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            GetButton((int)Buttons.OnlyOne).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
            GetButton((int)Buttons.Zombie).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 3));
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
                    Debug.Log("Marathon");
                    selectedSprite = _marathonSp;
                    _mapname = _marathon;
                    break;
                case 2:
                    Debug.Log("OnlyOne");
                    selectedSprite = _onlyoneSp;
                    _mapname = _onlyone;
                    break;
                case 3:
                    Debug.Log("Zombie");
                    selectedSprite = _zombieSp;
                    _mapname = _zombie;
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
