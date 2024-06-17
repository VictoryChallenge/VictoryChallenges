using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.CL
{ 
    /// <summary>
    /// 여러 UI 요소를 바인딩하고 관리하는 클래스.
    /// </summary>
    public abstract class UI_Base : MonoBehaviour
    {
        public abstract void Init();

        // Type을 key로 가지고, object 배열을 갖는 dictionary 생성
        Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

        #region 바인딩 함수
        /// <summary>
        /// Enum에 있는 요소들을 자식 객체에서 찾아서 바인딩
        /// </summary>
        /// <typeparam name="T">바인딩할 유형</typeparam>
        /// <param name="type">바인딩할 Enum타입 이름</param>
        protected void Bind<T>(Type type) where T : UnityEngine.Object
        {
            // 열거형 타입 안에 들어있는 각 멤버의 갯수 만큼 배열로 반환
            string[] names = Enum.GetNames(type);

            // 열거형에 있는 멤버 수 만큼의 object 배열 생성 및 초기화
            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
            // T (Button이면 Button, TextMeshPro면 TextMeshPro 등 자료형) 를 objects 배열 안에 추가
            _objects.Add(typeof(T), objects);

            // 열거형 멤버 수 만큼 반복
            for (int i = 0; i < names.Length; i++)
            {
                // T가 GameObject인 경우 해당 이름의 자식 오브젝트를 찾아 배열에 저장. gameObject는 this와 같음.
                if (typeof(T) == typeof(GameObject))
                    objects[i] = Util.FindChild(gameObject, names[i], true);
                // GameObject가 아닌 다른 타입일 경우 해당 타입의 컴포넌트가 붙은 오브젝트를 찾아 저장.
                else
                    objects[i] = Util.FindChild<T>(gameObject, names[i], true);
            }
        }
        #endregion 바인딩 끝

        #region UI 등 컴포넌트 요소 가져오는 함수
        /// <summary>
        /// 맨 위에서 선언한 object 딕셔너리에서 해당 인덱스의 요소를 가져오는 함수
        /// </summary>
        /// <typeparam name="T">  </typeparam>
        /// <param name="idx"> 인덱스 </param>
        /// <returns></returns>
        protected T Get<T>(int idx) where T : UnityEngine.Object
        {
            // 해당 유형의 값을 저장할 배열 선언 및 초기화
            UnityEngine.Object[] objects = null;
            
            // 딕셔너리에서 내가 찾는 T타입(key)에 해당하는 배열(value)을 가져오기, false면 null 반환
            if (_objects.TryGetValue(typeof(T), out objects) == false)
                return null;

            // true라면 해당 배열에서 인덱스가 idx인 요소를 반환.
            // as를 사용하여 형변환 실패 시 null 반환.
            return objects[idx] as T;
        }

        // Get 함수들로 해당 타입의 인덱스(idx)에 해당하는 요소를 가져옴.
        protected TextMeshProUGUI GetTextMeshPro(int idx) { return Get<TextMeshProUGUI>(idx); }
        protected Text GetText(int idx) { return Get<Text>(idx); }
        protected Sprite GetSprite(int idx) { return Get<Sprite>(idx); }
        protected Button GetButton(int idx) { return Get<Button>(idx); }
        protected Image GetImage(int idx) { return Get<Image>(idx); }
        protected Slider GetSlider(int idx) { return Get<Slider>(idx); }
        protected TMP_Dropdown GetDropdown(int idx) { return Get<TMP_Dropdown>(idx); }
        protected Toggle GetToggle(int idx) { return Get<Toggle>(idx); }
        protected TMP_InputField GetInputField(int idx) { return Get<TMP_InputField>(idx);}
        protected ScrollRect GetScrollRect(int idx) { return Get<ScrollRect>(idx); }
        #endregion 끝

        #region 이벤트 핸들러 작업 (AddListener과 동일한 기능)
        /// <summary>
        /// UI 이벤트를 추가하는 함수
        /// </summary>
        /// <param name="go"> UI 이벤트를 추가할 게임오브젝트 </param>
        /// <param name="action"> 이벤트 핸들러로 등록할 PointerEventData를 매개변수로 받는 Action </param>
        /// <param name="type"> 기본 값은 Click이며, 추가할 UI 이벤트의 종류 (Define에 정의) </param>
        public static void AddUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
        {
            // 게임오브젝트에 UI_EventHandler 컴포넌트를 가져오거나 추가함.
            UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

            // UI 이벤트의 종류에 따라 액션을 해당 이벤트 핸들러에 등록
            switch (type)
            {
                // Click인 경우, 기존의 클릭핸들러를 제거하고 새로운 action을 등록
                case Define.UIEvent.Click:
                    evt.OnClickHandler -= action;
                    evt.OnClickHandler += action;
                    break;
                // Drag인 경우, 기존의 드래그핸들러를 제거하고 새로운 action을 등록
                case Define.UIEvent.Drag:
                    evt.OnDragHandler -= action;
                    evt.OnDragHandler += action;
                    break;
            }
        }
        #endregion 끝
    }
}