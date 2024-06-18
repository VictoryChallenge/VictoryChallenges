using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.CL
{
    /// <summary>
    /// 전반적인 UI를 관리하는 클래스
    /// </summary>
    public class UIManager
    {
        // UI 요소들의 레이어 순서를 결정하는 int 변수
        int _order = 10;

        // 팝업 UI들을 담을 stack
        Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
        // 현재 활성화 된 Scene UI
        UI_Scene _sceneUI = null;

        /// <summary>
        /// UI 요소들을 담을 루트 객체
        /// </summary>
        public GameObject Root
        {
            get
            {
                GameObject root = GameObject.Find("UI_Root");
                // 루트 객체가 없으면 새로 생성
                if (root == null)
                    root = new GameObject { name = "UI_Root" };
                return root;
            }
        }

        /// <summary>
        /// Canvas를 설정하는 함수
        /// </summary>
        /// <param name="go"> 캔버스를 설정할 게임 오브젝트 </param>
        /// <param name="sort"> 레이어 순서를 정할 bool 값으로 기본값은 true </param>
        public void SetCanvas(GameObject go, bool sort = true)
        {
            // 게임오브젝트에 Canvas 컴포넌트를 붙이거나 가져옴
            Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
            GraphicRaycaster GR = Util.GetOrAddComponent<GraphicRaycaster>(go);
            CanvasScaler CS = Util.GetOrAddComponent<CanvasScaler>(go);
            // Rednermode는 ScreenSpaceOverlay로 설정
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            CS.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            CS.referenceResolution = new Vector2(1920, 1080);
            CS.matchWidthOrHeight = 0.5f;

            // sort가 true라면
            if (sort)
            {
                // 레이어 순서를 증가시킴 (sortingorder++)
                canvas.sortingOrder = _order;
                _order++;
            }

            // false라면
            else
            {
                // 가장 처음 렌더링할 캔버스로 설정
                canvas.sortingOrder = 0;
            }
        }

        #region 씬관련
        /// <summary>
        /// Scene UI를 표시하는 함수
        /// </summary>
        /// <typeparam name="T"> UI_Scene 타입을 갖는 T 매개변수 </typeparam>
        /// <param name="name"> 기본값이 null인 string </param>
        /// <returns></returns>
        public T ShowSceneUI<T>(string name = null) where T : UI_Scene
        {
            // 이름이 지정되지 않았을 경우 클래스의 이름을 사용
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            // UI 프리팹을 인스턴스화 하여 생성
            GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");

            // Scene UI를 가져오거나 추가
            T SceneUI = Util.GetOrAddComponent<T>(go);
            _sceneUI = SceneUI;

            // Root에 Scene UI를 추가
            go.transform.SetParent(Root.transform);

            // SceneUI 반환
            return SceneUI;
        }
        #endregion 씬관련 끝

        #region 팝업관련
        /// <summary>
        /// Popup UI를 표시하는 함수
        /// </summary>
        /// <typeparam name="T"> UI_Popup 타입을 갖는 T 매개변수 </typeparam>
        /// <param name="name"> 기본값은 null인 string </param>
        /// <returns></returns>
        public T ShowPopupUI<T>(string name = null) where T : UI_Popup
        {
            // 이름이 지정되지 않았을 경우 클래스 이름을 사용
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            // UI 프리팹을 인스턴스화 하여 생성
            GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");

            // Popup_UI 컴포넌트를 가져오거나 추가
            T popup = Util.GetOrAddComponent<T>(go);
            _popupStack.Push(popup);

            // 루트에 Popup UI를 추가
            go.transform.SetParent(Root.transform);

            return popup;
        }

        /// <summary>
        /// 특정 PopupUI를 닫는 함수
        /// </summary>
        /// <param name="popup"> 닫을 팝업 </param>
        public void ClosePopupUI(UI_Popup popup)
        {
            // 팝업 스택이 비어있으면 종료
            if (_popupStack.Count == 0)
                return;

            // 최상위 팝업이 닫으려는 팝업과 일치하지 않으면 종료
            if (_popupStack.Peek() != popup)
            {
                Debug.Log("Close Popup Failed");
                return;
            }

            // 매개변수가 없는 팝업 닫는 함수 호출
            ClosePopupUI();
        }

        /// <summary>
        /// 가장 최상위 팝업을 닫는 함수
        /// </summary>
        public void ClosePopupUI()
        {
            // 팝업 스택이 비어있으면 종료
            if (_popupStack.Count == 0)
                return;

            // 가장 최상위의 팝업 닫기
            UI_Popup popup = _popupStack.Pop();
            Managers.Resource.Destroy(popup.gameObject);
            popup = null;

            // 레이어 순서 감소
            _order--;
        }

        // 모든 Popup UI를 닫는 함수
        public void CloseAllPopupUI()
        {
            // 팝업스택이 빌 때까지 팝업을 닫기
            while (_popupStack.Count > 0)
                ClosePopupUI();
        }
        #endregion 팝업관련 끝

        public bool IsPointerOverUIElement()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            // Raycast 결과 중 UI 레이어에 해당하는 요소가 있는지 확인
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.layer == 5)
                {
                    return true;
                }
            }
            return false;
        }
    }
}