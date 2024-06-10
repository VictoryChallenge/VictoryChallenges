using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VictoryChallenge.Scripts.CL
{ 
    /// <summary>
    /// 이벤트 핸들러를 처리하는 class로 드래그핸들러와 클릭핸들러에 관한 처리를 하고있음.
    /// </summary>
    public class UI_EventHandler : MonoBehaviour, IDragHandler, IPointerClickHandler
    {
        // click과 drag에 대한 액션 대리자 생성
        public Action<PointerEventData> OnClickHandler = null;
        public Action<PointerEventData> OnDragHandler = null;

        public void OnPointerClick(PointerEventData eventData)
        {
            // 클릭 이벤트 핸들러가 등록되어있다면 등록된 이벤트를 실행
            if (OnClickHandler != null)
                OnClickHandler.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // 드래그 이벤트 핸들러가 등록되어있다면 등록된 이벤트를 실행
            if (OnDragHandler != null)
                OnDragHandler.Invoke(eventData);
        }
    }
}