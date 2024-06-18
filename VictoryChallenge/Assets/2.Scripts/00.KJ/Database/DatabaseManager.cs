using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

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

        public void SaveUserDB(FirebaseUser user)
        {
            if (gameData == null || !gameData.users.ContainsKey(user.UserId))
            {
                Debug.LogError("사용자 데이터가 초기화 되지 않았거나 데이터가 없습니다.");
            }

            User userData = gameData.users[user.UserId];
            string jsonData = JsonUtility.ToJson(userData);
            string shortUID = UIDHelper.GenerateShortUID(user.UserId);
            WriteUserData(shortUID, true, jsonData);

        }

        public IEnumerator LoadUserDB(FirebaseUser user, string jsonData)
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
                    string json = snapshot.Child("jsonData").Value.ToString();
                    User loadUser = JsonUtility.FromJson<User>(json);
                    gameData.users[shortUID] = loadUser;
                }
            }
        }

        public void ReadUserData(string userkey, Action callback = null)
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

                    string strUserData = snapshot.Value.ToString();
                    if (!string.IsNullOrEmpty(strUserData))
                    {
                        Debug.Log("복원 완료" + gameData);
                        gameData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameData>(strUserData);
                    }

                    if (callback != null)
                    {
                        callback?.Invoke();
                    }
                }
            });
        }

        public void WriteUserData(string userkey, bool isLoggedIn, string jsonData)
        {
            DatabaseReference db = null;
            db = FirebaseDatabase.DefaultInstance.GetReference("User");

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("userkey", userkey);
            dic.Add("isLoggedIn", isLoggedIn);
            dic.Add("jsonData", jsonData);

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add(userkey, dic);

            db.UpdateChildrenAsync(data).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Database Update");
                }
            });
        }
    }
}

