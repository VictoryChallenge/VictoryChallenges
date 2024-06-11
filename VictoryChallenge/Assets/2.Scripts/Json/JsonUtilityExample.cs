using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VictoryChallenge.Json.JsonExample;

namespace VictoryChallenge.Json
{
    public class JsonUtilityExample : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //JsonTestClass jTest1 = new JsonTestClass();
            //string jsonData = JsonUtility.ToJson(jTest1);
            //Debug.Log(jsonData);

            //JsonTestClass jTest2 = JsonUtility.FromJson<JsonTestClass>(jsonData);
            //jTest2.Print();

            //JsonVector jVector = new JsonVector();
            //string jsonData = JsonUtility.ToJson(jVector);
            //Debug.Log(jsonData);

            GameObject obj1 = new GameObject();
            var test1 = obj1.AddComponent<TestMono>();
            test1.i = 100;
            test1.v3 /= 10;
            string jsonData = JsonUtility.ToJson(obj1.GetComponent<TestMono>());
            Debug.Log(jsonData);

            GameObject obj2 = new GameObject();
            JsonUtility.FromJsonOverwrite(jsonData, obj2.AddComponent<TestMono>());
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

}
