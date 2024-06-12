using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Extensions;

[System.Serializable]
public class User
{
    public string name;
    public string nickname;
    public int timestamp;
    // 프로퍼티는 지원이 안된다고 함.

    public User(string name, string nickname, int timestamp)
    {
        this.name = name;
        this.nickname = nickname;
        this.timestamp = timestamp;
    }
}

public class TestJson : MonoBehaviour
{
    DatabaseReference reference;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                reference = FirebaseDatabase.DefaultInstance.RootReference;

                User user = new User("김채림", "체리미", 1574940551);
                string json = JsonUtility.ToJson(user);

                string key = reference.Child("user").Push().Key;

                reference.Child("user").Child(key).SetRawJsonValueAsync(json);
            }

            else 
            {
                Debug.LogError("Could not resolve all firebase dependencies : " + task.Result);
            }
        });
    }
}
