using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Scripts.CL
{ 
    public class KeyGuidePopup : UI_Popup
    {
        void Start()
        {
            Init();
            Cursor.visible = true;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public override void Init()
        {
            base.Init();
        }

        void Update()
        {

        }
    }
}
