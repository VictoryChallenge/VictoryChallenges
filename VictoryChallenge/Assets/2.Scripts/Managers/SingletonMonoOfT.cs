using System;
using UnityEngine;

namespace VictoryChallenge.Managers
{
    /// <summary>
    /// MonoBehaviour를 상속받는 싱글톤
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonMonoBase<T> : MonoBehaviour
        where T : SingletonMonoBase<T>
    {
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
                return _instance;
            }
        }
        private static T _instance;

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}

public class SingletonLazy<T> : MonoBehaviour where T : class
{
    private static readonly Lazy<T> _instance = 
        new Lazy<T>(() =>
        {
            T instance = FindObjectOfType(typeof(T)) as T;

            if (instance == null)
            {
                GameObject obj = new GameObject("SingletonLazy");
                instance = obj.AddComponent(typeof(T)) as T;

                DontDestroyOnLoad(obj);
            }
            else
            {
                Destroy(instance as GameObject);
            }

            return instance;
        });

    public static T Instance
    {
        get
        {
            return _instance.Value;
        }
    }
}
