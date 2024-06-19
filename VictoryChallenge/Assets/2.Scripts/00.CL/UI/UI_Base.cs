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
    /// ���� UI ��Ҹ� ���ε��ϰ� �����ϴ� Ŭ����.
    /// </summary>
    public abstract class UI_Base : MonoBehaviour
    {
        public abstract void Init();

        // Type�� key�� ������, object �迭�� ���� dictionary ����
        Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

        #region ���ε� �Լ�
        /// <summary>
        /// Enum�� �ִ� ��ҵ��� �ڽ� ��ü���� ã�Ƽ� ���ε�
        /// </summary>
        /// <typeparam name="T">���ε��� ����</typeparam>
        /// <param name="type">���ε��� EnumŸ�� �̸�</param>
        protected void Bind<T>(Type type) where T : UnityEngine.Object
        {
            // ������ Ÿ�� �ȿ� ����ִ� �� ����� ���� ��ŭ �迭�� ��ȯ
            string[] names = Enum.GetNames(type);

            // �������� �ִ� ��� �� ��ŭ�� object �迭 ���� �� �ʱ�ȭ
            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
            // T (Button�̸� Button, TextMeshPro�� TextMeshPro �� �ڷ���) �� objects �迭 �ȿ� �߰�
            _objects.Add(typeof(T), objects);

            // ������ ��� �� ��ŭ �ݺ�
            for (int i = 0; i < names.Length; i++)
            {
                // T�� GameObject�� ��� �ش� �̸��� �ڽ� ������Ʈ�� ã�� �迭�� ����. gameObject�� this�� ����.
                if (typeof(T) == typeof(GameObject))
                    objects[i] = Util.FindChild(gameObject, names[i], true);
                // GameObject�� �ƴ� �ٸ� Ÿ���� ��� �ش� Ÿ���� ������Ʈ�� ���� ������Ʈ�� ã�� ����.
                else
                    objects[i] = Util.FindChild<T>(gameObject, names[i], true);
            }
        }
        #endregion ���ε� ��

        #region UI �� ������Ʈ ��� �������� �Լ�
        /// <summary>
        /// �� ������ ������ object ��ųʸ����� �ش� �ε����� ��Ҹ� �������� �Լ�
        /// </summary>
        /// <typeparam name="T">  </typeparam>
        /// <param name="idx"> �ε��� </param>
        /// <returns></returns>
        protected T Get<T>(int idx) where T : UnityEngine.Object
        {
            // �ش� ������ ���� ������ �迭 ���� �� �ʱ�ȭ
            UnityEngine.Object[] objects = null;
            
            // ��ųʸ����� ���� ã�� TŸ��(key)�� �ش��ϴ� �迭(value)�� ��������, false�� null ��ȯ
            if (_objects.TryGetValue(typeof(T), out objects) == false)
                return null;

            // true��� �ش� �迭���� �ε����� idx�� ��Ҹ� ��ȯ.
            // as�� ����Ͽ� ����ȯ ���� �� null ��ȯ.
            return objects[idx] as T;
        }

        // Get �Լ���� �ش� Ÿ���� �ε���(idx)�� �ش��ϴ� ��Ҹ� ������.
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
        #endregion ��

        #region �̺�Ʈ �ڵ鷯 �۾� (AddListener�� ������ ���)
        /// <summary>
        /// UI �̺�Ʈ�� �߰��ϴ� �Լ�
        /// </summary>
        /// <param name="go"> UI �̺�Ʈ�� �߰��� ���ӿ�����Ʈ </param>
        /// <param name="action"> �̺�Ʈ �ڵ鷯�� ����� PointerEventData�� �Ű������� �޴� Action </param>
        /// <param name="type"> �⺻ ���� Click�̸�, �߰��� UI �̺�Ʈ�� ���� (Define�� ����) </param>
        public static void AddUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
        {
            // ���ӿ�����Ʈ�� UI_EventHandler ������Ʈ�� �������ų� �߰���.
            UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

            // UI �̺�Ʈ�� ������ ���� �׼��� �ش� �̺�Ʈ �ڵ鷯�� ���
            switch (type)
            {
                // Click�� ���, ������ Ŭ���ڵ鷯�� �����ϰ� ���ο� action�� ���
                case Define.UIEvent.Click:
                    evt.OnClickHandler -= action;
                    evt.OnClickHandler += action;
                    break;
                // Drag�� ���, ������ �巡���ڵ鷯�� �����ϰ� ���ο� action�� ���
                case Define.UIEvent.Drag:
                    evt.OnDragHandler -= action;
                    evt.OnDragHandler += action;
                    break;
            }
        }
        #endregion ��
    }
}