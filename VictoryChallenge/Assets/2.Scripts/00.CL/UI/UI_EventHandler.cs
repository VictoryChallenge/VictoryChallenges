using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VictoryChallenge.Scripts.CL
{ 
    /// <summary>
    /// �̺�Ʈ �ڵ鷯�� ó���ϴ� class�� �巡���ڵ鷯�� Ŭ���ڵ鷯�� ���� ó���� �ϰ�����.
    /// </summary>
    public class UI_EventHandler : MonoBehaviour, IDragHandler, IPointerClickHandler
    {
        // click�� drag�� ���� �׼� �븮�� ����
        public Action<PointerEventData> OnClickHandler = null;
        public Action<PointerEventData> OnDragHandler = null;

        public void OnPointerClick(PointerEventData eventData)
        {
            // Ŭ�� �̺�Ʈ �ڵ鷯�� ��ϵǾ��ִٸ� ��ϵ� �̺�Ʈ�� ����
            if (OnClickHandler != null)
                OnClickHandler.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // �巡�� �̺�Ʈ �ڵ鷯�� ��ϵǾ��ִٸ� ��ϵ� �̺�Ʈ�� ����
            if (OnDragHandler != null)
                OnDragHandler.Invoke(eventData);
        }
    }
}