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

    #region UID 해시함수
    /// <summary>
    /// UID를 해시함수를 이용해서 ShortUID로 변환
    /// </summary>
    public static class UIDHelper
    {
        public static string GenerateShortUID(string longUID)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // SHA256 해시 값을 계산
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(longUID));

                // 바이트 배열을 String으로 변환
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                // 해시값의 앞부분만(8자리) 사용하여 ShortUID 생성
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
                Debug.Log("gameData 초기화");
            }
        }

        public void SaveUserDB(FirebaseUser user)
        {
            if (gameData == null || !gameData.users.ContainsKey(user.UserId))
            {
                Debug.LogError("사용자 데이터가 초기화 되지 않았거나 데이터가 없습니다.");
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
                Debug.LogError("데이터 가져오는 도중 오류 발생 " + task.Exception);
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

            // userkey를 가진 User의 데이터를 gameData에 넣기
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
                    Debug.Log("로그아웃 성공");
                }
            });
                    //게임나가기
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

