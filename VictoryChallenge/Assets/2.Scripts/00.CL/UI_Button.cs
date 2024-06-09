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
            // �� Enum Ÿ���� ���ε�
            Bind<Button>(typeof(Buttons));
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<GameObject>(typeof(GameObjects));
            // Bind<Image>(typeof(Images));

            // Ȯ���Լ� �̻�� ��
            // UI_Base.AddUIEvent(GetButton((int)Buttons.PointButton).gameObject, OnButtonClicked);
            // Ȯ���Լ� ��� ��
            GetButton((int)Buttons.PointButton).gameObject.AddUIEvent(OnButtonClicked);

            // ItemIcon�̶�� �̹����� �����ͼ� go ������Ʈ�� �Ҵ�
            // GameObject go = GetImage((int)Images.ItemIcon).gameObject;
            // ���ٽ��� �̿��Ͽ� �̹��� drag�� ��Ÿ��
            // AddUIEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);
        }

        int _score = 0;

        public void OnButtonClicked(PointerEventData data)
        {
            _score++;
            GetTextMeshPro((int)Texts.ScoreText).text = $"����: {_score}";
        }
    }
}