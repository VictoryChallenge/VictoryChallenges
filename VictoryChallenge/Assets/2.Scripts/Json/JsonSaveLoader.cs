using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using static VictoryChallenge.Json.JsonExample;

public class JsonSaveLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Save
        //FileStream stream = new FileStream(Application.dataPath + "/test.json", FileMode.OpenOrCreate);
        //JsonTestClass jTest1 = new JsonTestClass();
        //string JsonData = JsonConvert.SerializeObject(jTest1);
        //byte[] data = Encoding.UTF8.GetBytes(JsonData);
        //stream.Write(data, 0, data.Length);
        //stream.Close();

        // Load
        FileStream stream = new FileStream(Application.dataPath + "/test.json", FileMode.Open);
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        stream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        JsonTestClass jTest2 = JsonConvert.DeserializeObject<JsonTestClass>(jsonData);
        jTest2.Print();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
