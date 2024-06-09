using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using VictoryChallenge.Scripts.CL;
using TMPro;

namespace VictoryChallenge.Scripts.CL
{ 
    public class UI_Button : UI_Base
    {
        enum Buttons
        {
            PointButton,
        }

        enum Texts
        {
            PointText,
            ScoreText
        }

        enum GameObjects
        {
            TestObject,
        }

        enum Images
        {
            ItemIcon,
        }

        private void Start()
        {
            // 각 Enum 타입을 바인딩
            Bind<Button>(typeof(Buttons));
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<GameObject>(typeof(GameObjects));
            // Bind<Image>(typeof(Images));

            // 확장함수 미사용 시
            // UI_Base.AddUIEvent(GetButton((int)Buttons.PointButton).gameObject, OnButtonClicked);
            // 확장함수 사용 시
            GetButton((int)Buttons.PointButton).gameObject.AddUIEvent(OnButtonClicked);

            // ItemIcon이라는 이미지를 가져와서 go 오브젝트에 할당
            // GameObject go = GetImage((int)Images.ItemIcon).gameObject;
            // 람다식을 이용하여 이미지 drag를 나타냄
            // AddUIEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);
        }

        int _score = 0;

        public void OnButtonClicked(PointerEventData data)
        {
            _score++;
            GetTextMeshPro((int)Texts.ScoreText).text = $"점수: {_score}";
        }
    }
}