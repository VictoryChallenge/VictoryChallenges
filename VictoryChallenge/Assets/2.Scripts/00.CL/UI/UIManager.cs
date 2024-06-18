using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.CL
{
    /// <summary>
    /// �������� UI�� �����ϴ� Ŭ����
    /// </summary>
    public class UIManager
    {
        // UI ��ҵ��� ���̾� ������ �����ϴ� int ����
        int _order = 10;

        // �˾� UI���� ���� stack
        Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
        // ���� Ȱ��ȭ �� Scene UI
        UI_Scene _sceneUI = null;

        /// <summary>
        /// UI ��ҵ��� ���� ��Ʈ ��ü
        /// </summary>
        public GameObject Root
        {
            get
            {
                GameObject root = GameObject.Find("UI_Root");
                // ��Ʈ ��ü�� ������ ���� ����
                if (root == null)
                    root = new GameObject { name = "UI_Root" };
                return root;
            }
        }

        /// <summary>
        /// Canvas�� �����ϴ� �Լ�
        /// </summary>
        /// <param name="go"> ĵ������ ������ ���� ������Ʈ </param>
        /// <param name="sort"> ���̾� ������ ���� bool ������ �⺻���� true </param>
        public void SetCanvas(GameObject go, bool sort = true)
        {
            // ���ӿ�����Ʈ�� Canvas ������Ʈ�� ���̰ų� ������
            Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
            GraphicRaycaster GR = Util.GetOrAddComponent<GraphicRaycaster>(go);
            CanvasScaler CS = Util.GetOrAddComponent<CanvasScaler>(go);
            // Rednermode�� ScreenSpaceOverlay�� ����
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            CS.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            CS.referenceResolution = new Vector2(1920, 1080);
            CS.matchWidthOrHeight = 0.5f;

            // sort�� true���
            if (sort)
            {
                // ���̾� ������ ������Ŵ (sortingorder++)
                canvas.sortingOrder = _order;
                _order++;
            }

            // false���
            else
            {
                // ���� ó�� �������� ĵ������ ����
                canvas.sortingOrder = 0;
            }
        }

        #region ������
        /// <summary>
        /// Scene UI�� ǥ���ϴ� �Լ�
        /// </summary>
        /// <typeparam name="T"> UI_Scene Ÿ���� ���� T �Ű����� </typeparam>
        /// <param name="name"> �⺻���� null�� string </param>
        /// <returns></returns>
        public T ShowSceneUI<T>(string name = null) where T : UI_Scene
        {
            // �̸��� �������� �ʾ��� ��� Ŭ������ �̸��� ���
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            // UI �������� �ν��Ͻ�ȭ �Ͽ� ����
            GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");

            // Scene UI�� �������ų� �߰�
            T SceneUI = Util.GetOrAddComponent<T>(go);
            _sceneUI = SceneUI;

            // Root�� Scene UI�� �߰�
            go.transform.SetParent(Root.transform);

            // SceneUI ��ȯ
            return SceneUI;
        }
        #endregion ������ ��

        #region �˾�����
        /// <summary>
        /// Popup UI�� ǥ���ϴ� �Լ�
        /// </summary>
        /// <typeparam name="T"> UI_Popup Ÿ���� ���� T �Ű����� </typeparam>
        /// <param name="name"> �⺻���� null�� string </param>
        /// <returns></returns>
        public T ShowPopupUI<T>(string name = null) where T : UI_Popup
        {
            // �̸��� �������� �ʾ��� ��� Ŭ���� �̸��� ���
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            // UI �������� �ν��Ͻ�ȭ �Ͽ� ����
            GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");

            // Popup_UI ������Ʈ�� �������ų� �߰�
            T popup = Util.GetOrAddComponent<T>(go);
            _popupStack.Push(popup);

            // ��Ʈ�� Popup UI�� �߰�
            go.transform.SetParent(Root.transform);

            return popup;
        }

        /// <summary>
        /// Ư�� PopupUI�� �ݴ� �Լ�
        /// </summary>
        /// <param name="popup"> ���� �˾� </param>
        public void ClosePopupUI(UI_Popup popup)
        {
            // �˾� ������ ��������� ����
            if (_popupStack.Count == 0)
                return;

            // �ֻ��� �˾��� �������� �˾��� ��ġ���� ������ ����
            if (_popupStack.Peek() != popup)
            {
                Debug.Log("Close Popup Failed");
                return;
            }

            // �Ű������� ���� �˾� �ݴ� �Լ� ȣ��
            ClosePopupUI();
        }

        /// <summary>
        /// ���� �ֻ��� �˾��� �ݴ� �Լ�
        /// </summary>
        public void ClosePopupUI()
        {
            // �˾� ������ ��������� ����
            if (_popupStack.Count == 0)
                return;

            // ���� �ֻ����� �˾� �ݱ�
            UI_Popup popup = _popupStack.Pop();
            Managers.Resource.Destroy(popup.gameObject);
            popup = null;

            // ���̾� ���� ����
            _order--;
        }

        // ��� Popup UI�� �ݴ� �Լ�
        public void CloseAllPopupUI()
        {
            // �˾������� �� ������ �˾��� �ݱ�
            while (_popupStack.Count > 0)
                ClosePopupUI();
        }
        #endregion �˾����� ��

        public bool IsPointerOverUIElement()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            // Raycast ��� �� UI ���̾ �ش��ϴ� ��Ұ� �ִ��� Ȯ��
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