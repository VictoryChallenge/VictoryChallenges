using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VictoryChallenge.Scripts.CL;

namespace VictoryChallenge.Scripts.CL
{ 
    // 코드의 가독성을 위한 확장 클래스
    public static class Extension
    {
        // 확장클래스를 씀으로써 인스턴스화 하여 쓸 수 있다.
        public static void AddUIEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
        {
            UI_Base.AddUIEvent(go, action, type);
        }
    }
}