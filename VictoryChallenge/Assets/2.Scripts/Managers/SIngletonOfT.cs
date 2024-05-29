using UnityEngine;
using System;

namespace RiskOfRain2.Managers
{
    /// <summary>
    /// MonoBehaviour를 상속받지 않는 싱글톤
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBase<T>
        where T : SingletonBase<T>
    {
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    //Type type = typeof(T);
                    //ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { });
                    //_instance = (T)constructorInfo.Invoke(null);

                    _instance = Activator.CreateInstance<T>();
                    _instance.Init();
                }
                return _instance;
            }
        }
        private static T _instance;

        protected virtual void Init()
        {
        }
    }
}