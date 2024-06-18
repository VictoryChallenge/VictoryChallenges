using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using VictoryChallenge.Customize;
using VictoryChallenge.KJ.Auth;

namespace VictoryChallenge.KJ.Database
{

    #region UID �ؽ��Լ�
    /// <summary>
    /// UID�� �ؽ��Լ��� �̿��ؼ� ShortUID�� ��ȯ
    /// </summary>
    public static class UIDHelper
    {
        public static string GenerateShortUID(string longUID)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // SHA256 �ؽ� ���� ���
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(longUID));

                // ����Ʈ �迭�� String���� ��ȯ
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                // �ؽð��� �պκи�(8�ڸ�) ����Ͽ� ShortUID ����
                return builder.ToString().Substring(0, 8);
            }
        }
    }
    #endregion

    public class DatabaseManager : SingletonLazy<DatabaseManager>
    {
        public GameData gameData { get; private set; }

        public DatabaseManager()
        {
            if (gameData == null)
            {
                gameData = new GameData();
                Debug.Log("gameData �ʱ�ȭ");
            }
        }

        public void SaveUserDB(FirebaseUser user)
        {
            if (gameData == null || !gameData.users.ContainsKey(user.UserId))
            {
                Debug.LogError("����� �����Ͱ� �ʱ�ȭ ���� �ʾҰų� �����Ͱ� �����ϴ�.");
            }

            User userData = gameData.users[user.UserId]; 
            string jsonData = JsonUtility.ToJson(userData);
            string shortUID = UIDHelper.GenerateShortUID(user.UserId);

            PlayerCharacterCustomized playerData = new PlayerCharacterCustomized();
            string customData = playerData.Initialize();

            WriteUserData(shortUID, jsonData, customData);
        }

        public IEnumerator LoadUserDB(FirebaseUser user)
        {
            if (gameData == null)
            {
                gameData = new GameData();
            }

            string shortUID = UIDHelper.GenerateShortUID(user.UserId);
            DatabaseReference db = FirebaseDatabase.DefaultInstance.GetReference("User").Child(shortUID);

            var task = db.GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogError("������ �������� ���� ���� �߻� " + task.Exception);
            }
            else
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string json = snapshot.Children.ToString();
                    User loadUser = JsonUtility.FromJson<User>(json);
                    gameData.users[shortUID] = loadUser;
                }
            }
        }

        public void ReadUserData(string shortUID, Action callback = null)
        {
            DatabaseReference db = FirebaseDatabase.DefaultInstance.GetReference("User");
            db.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("ReadData is Faulted");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log("ChilderenCount" + snapshot.ChildrenCount);

                    foreach (var child in snapshot.Children)
                    {
                        if(child.Key == shortUID)
                        {
                            Debug.Log("child.Value.ToString() : " + child.ToString());
                            string strData = child.Child("jsonData").Value.ToString();
                            User loadUser = JsonUtility.FromJson<User>(strData);
                            gameData.users[shortUID] = loadUser;
                            Debug.Log("gameData" + gameData.users[shortUID]);
                        }
                    }

                    if (callback != null)
                    {
                        callback?.Invoke();
                    }
                }
            });
        }

        public void WriteUserData(string userkey, string jsonData, string customData)
        {
            DatabaseReference db = null;
            db = FirebaseDatabase.DefaultInstance.GetReference("User");

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("userkey", userkey);
            dic.Add("jsonData", jsonData);
            dic.Add("customData", customData);

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add(userkey, dic);

            db.UpdateChildrenAsync(data).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Database Update");
                }
            });

            // userkey�� ���� User�� �����͸� gameData�� �ֱ�
            ReadUserData(userkey);
        }

        public void SignOutProcess(string userkey, string jsonData, string customData)
        {
            DatabaseReference db = null;
            db = FirebaseDatabase.DefaultInstance.GetReference("User");

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("userkey", userkey);
            dic.Add("jsonData", jsonData);
            dic.Add("customData", customData);

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add(userkey, dic);

            db.UpdateChildrenAsync(data).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Database Update");

                    Authentication.Instance._auth.SignOut();
                    Debug.Log("�α׾ƿ� ����");
                }
            });
                    //���ӳ�����
                    Application.Quit();
        }
            public string GetUserJsonData(FirebaseUser user)
            {
                User userData = gameData.users[user.UserId];
                string jsonData = JsonUtility.ToJson(userData);
                return jsonData;
            }
        
    }
}

