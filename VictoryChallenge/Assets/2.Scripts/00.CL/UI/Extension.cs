using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VictoryChallenge.Scripts.CL;

namespace VictoryChallenge.Scripts.CL
{ 
    // �ڵ��� �������� ���� Ȯ�� Ŭ����
    public static class Extension
    {
        // Ȯ��Ŭ������ �����ν� �ν��Ͻ�ȭ �Ͽ� �� �� �ִ�.
        public static void AddUIEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
        {
            UI_Base.AddUIEvent(go, action, type);
        }
    }
}