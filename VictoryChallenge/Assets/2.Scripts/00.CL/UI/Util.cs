using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Scripts.CL
{
    public class Util
    {
        /// <summary>
        /// 컴포넌트가 없으면 추가하고, 있으면 그대로 반환하는 함수
        /// </summary>
        /// <typeparam name="T"> 찾고자 하는 타입 </typeparam>
        /// <param name="go"> 컴포넌트가 붙어있는/붙이고자하는 오브젝트 </param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
        {
            T component = go.GetComponent<T>();

            // 만약 component가 없다면 추가
            if (component == null)
                component = go.AddComponent<T>();
            return component;
        }

        #region 자식들 중 원하는 오브젝트/컴포넌트 찾기
        /// <summary>
        /// 하위 객체 중 파라미터 "name"과 같은 이름을 가진 게임오브젝트를 반환하는 함수
        /// </summary>
        /// <param name="go"> 찾고자 하는 오브젝트의 부모 오브젝트. </param>
        /// <param name="name"> Enum 타입에 쓰여진 이름 </param>
        /// <param name="recursive"> 재귀적으로 탐색하여 가장 하위 오브젝트까지 찾을것인지 결정하는 bool 값 </param>
        /// <returns></returns>
        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            // FindChild<T>를 통해 재귀적으로 탐색
            Transform transform = FindChild<Transform>(go, name, recursive);
            
            if (transform == null)
                return null;

            return transform.gameObject;
        }

        /// <summary>
        /// 하위 객체 중 파라미터 "name"과 같은 이름을 가진 컴포넌트를 반환하는 함수
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"> 찾고자 하는 오브젝트의 부모 오브젝트. </param>
        /// <param name="name"> Enum 타입에 쓰여진 이름 </param>
        /// <param name="recursive"> 재귀적으로 탐색하여 가장 하위 오브젝트까지 찾을것인지 결정하는 bool 값 </param>
        /// <returns></returns>
        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
        {
            if (go == null)
                return null;

            // 재귀적 탐색을 하지 않을 때
            if (recursive == false)
            {
                // childCount는 직계 자식 오브젝트들을 찾아주는 함수.
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    // 각 자식 오브젝트들에 대한 transform을 가져옴
                    Transform transform = go.transform.GetChild(i);

                    // 만약 내가 찾는 name과 해당 자식 오브젝트의 name이 같다면
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        // T타입의 component를 가져오고, 반환.
                        T component = transform.GetComponent<T>();
                        if (component != null)
                            return component;
                    }
                }
            }

            // 재귀적 탐색을 할 때
            else
            {
                // 전체(직계 이외에도) 자식 오브젝트 중 T타입의 컴포넌트를 가진 자식을 검색
                foreach (T component in go.GetComponentsInChildren<T>())
                {
                    // 만약 그 자식오브젝트의 이름과 내가 찾고자 하는 name이 같다면 그 컴포넌트를 반환.
                    if (string.IsNullOrEmpty(name) || component.name == name)
                        return component;
                }
            }

            return null;
        }
        #endregion 찾기 끝
    }
}