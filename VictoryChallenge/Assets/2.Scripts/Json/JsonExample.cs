using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace VictoryChallenge.Json
{
    public class JsonExample : MonoBehaviour
    {
        void Start()
        {
            // 직렬화
            //JsonTestClass jTest1 = new JsonTestClass();
            //string jsonData = JsonConvert.SerializeObject(jTest1);
            //Debug.Log(jsonData);

            //JsonTestClass jTest2 = JsonConvert.DeserializeObject<JsonTestClass>(jsonData);
            //jTest2.Print();

            // 직렬화 오류
            //GameObject obj = new GameObject();
            //obj.AddComponent<TestMono>();
            //Debug.Log(JsonConvert.SerializeObject(obj.GetComponent<TestMono>()));

            // 직렬화 오류해결(비추천)
            JsonVector jsonVector = new JsonVector();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            Debug.Log(JsonConvert.SerializeObject(jsonVector, settings));
        }

        void Update()
        {
        
        }

        public class JsonVector
        {
            public Vector3 vector3 = new Vector3(1,1,1);
        }

        public class JsonTestClass
        {
            public int i;
            public float f;
            public bool b;
            public string str;
            public int[] iArray;
            public List<int> iList = new List<int>();
            public Dictionary<string, float> fDictionary = new Dictionary<string, float>();

            public JsonTestClass() 
            {
                i = 10;
                f = 99.9f;
                b = true;
                str = "Json Test string";
                iArray = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                for (int i = 0; i < 5; i++)
                {
                    iList.Add(i * 2);
                }

                fDictionary.Add("PIE", Mathf.PI);
                fDictionary.Add("Epsilon", Mathf.Epsilon);
                fDictionary.Add("Sqrt(2)", Mathf.Sqrt(2));
            }

            public void Print()
            {
                Debug.Log("i = " + i);
                Debug.Log("f = " + f);
                Debug.Log("b = " + b);
                Debug.Log("str = " + str);

                for(int i = 0; i < iArray.Length; i++)
                {
                    Debug.Log(string.Format("iArray[{0}] = {1}", i, iArray[i]));
                }

                for (int i = 0; i < iList.Count; i++)
                {
                    Debug.Log(string.Format("iList[{0}] = {1}", i, iList[i]));
                }

                foreach(var data in fDictionary)
                {
                    Debug.Log(string.Format("fDictionary[{0}] = {1}", data.Key, data.Value));
                }
            }
        }
    }
}
