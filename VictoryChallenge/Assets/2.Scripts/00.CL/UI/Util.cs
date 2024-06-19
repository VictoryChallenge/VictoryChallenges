using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Scripts.CL
{
    public class Util
    {
        /// <summary>
        /// ������Ʈ�� ������ �߰��ϰ�, ������ �״�� ��ȯ�ϴ� �Լ�
        /// </summary>
        /// <typeparam name="T"> ã���� �ϴ� Ÿ�� </typeparam>
        /// <param name="go"> ������Ʈ�� �پ��ִ�/���̰����ϴ� ������Ʈ </param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
        {
            T component = go.GetComponent<T>();

            // ���� component�� ���ٸ� �߰�
            if (component == null)
                component = go.AddComponent<T>();
            return component;
        }

        #region �ڽĵ� �� ���ϴ� ������Ʈ/������Ʈ ã��
        /// <summary>
        /// ���� ��ü �� �Ķ���� "name"�� ���� �̸��� ���� ���ӿ�����Ʈ�� ��ȯ�ϴ� �Լ�
        /// </summary>
        /// <param name="go"> ã���� �ϴ� ������Ʈ�� �θ� ������Ʈ. </param>
        /// <param name="name"> Enum Ÿ�Կ� ������ �̸� </param>
        /// <param name="recursive"> ��������� Ž���Ͽ� ���� ���� ������Ʈ���� ã�������� �����ϴ� bool �� </param>
        /// <returns></returns>
        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            // FindChild<T>�� ���� ��������� Ž��
            Transform transform = FindChild<Transform>(go, name, recursive);
            
            if (transform == null)
                return null;

            return transform.gameObject;
        }

        /// <summary>
        /// ���� ��ü �� �Ķ���� "name"�� ���� �̸��� ���� ������Ʈ�� ��ȯ�ϴ� �Լ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"> ã���� �ϴ� ������Ʈ�� �θ� ������Ʈ. </param>
        /// <param name="name"> Enum Ÿ�Կ� ������ �̸� </param>
        /// <param name="recursive"> ��������� Ž���Ͽ� ���� ���� ������Ʈ���� ã�������� �����ϴ� bool �� </param>
        /// <returns></returns>
        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
        {
            if (go == null)
                return null;

            // ����� Ž���� ���� ���� ��
            if (recursive == false)
            {
                // childCount�� ���� �ڽ� ������Ʈ���� ã���ִ� �Լ�.
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    // �� �ڽ� ������Ʈ�鿡 ���� transform�� ������
                    Transform transform = go.transform.GetChild(i);

                    // ���� ���� ã�� name�� �ش� �ڽ� ������Ʈ�� name�� ���ٸ�
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        // TŸ���� component�� ��������, ��ȯ.
                        T component = transform.GetComponent<T>();
                        if (component != null)
                            return component;
                    }
                }
            }

            // ����� Ž���� �� ��
            else
            {
                // ��ü(���� �̿ܿ���) �ڽ� ������Ʈ �� TŸ���� ������Ʈ�� ���� �ڽ��� �˻�
                foreach (T component in go.GetComponentsInChildren<T>())
                {
                    // ���� �� �ڽĿ�����Ʈ�� �̸��� ���� ã���� �ϴ� name�� ���ٸ� �� ������Ʈ�� ��ȯ.
                    if (string.IsNullOrEmpty(name) || component.name == name)
                        return component;
                }
            }

            return null;
        }
        #endregion ã�� ��
    }
}