using Newtonsoft.Json.Linq;
using Proyecto26;
using System;
using System.Collections;
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

    public class DBTutorial : SingletonLazy<DBTutorial>
    {
        public GameData gameData { get; private set; }
        public string customData { get; set; }
        public string userData { get; set; }

        [HideInInspector] public string dbUrl = "https://victorychallenge-b8854-default-rtdb.firebaseio.com/";


        public DBTutorial()
        {
            if (gameData == null)
            {
                gameData = new GameData();
                Debug.Log("gameData 초기화");
            }
        }

        public void SaveUserDB(string userId)
        {
            if (gameData == null || !gameData.users.ContainsKey(userId))
            {
                Debug.LogError("사용자 데이터가 초기화 되지 않았거나 데이터가 없습니다.");
            }

            User userData = gameData.users[userId];
            string jsonData = JsonUtility.ToJson(userData);
            string shortUID = UIDHelper.GenerateShortUID(userId);

            PlayerCharacterCustomized playerData = new PlayerCharacterCustomized();
            string customData = playerData.Initialize();

            WriteUserData(shortUID, jsonData, customData);
        }

        public IEnumerator LoadUserDB(string userId)
        {
            if (gameData == null)
            {
                gameData = new GameData();
            }

            string shortUID = UIDHelper.GenerateShortUID(userId);

            RestAPIAuth.Instance.EnsureIdToken(success =>
            {
                if (success)
                {
                    string idToken = RestAPIAuth.Instance.IdToken;

                    var request = new RequestHelper
                    {
                        Uri = $"{RestAPIAuth.Instance.dbUrl}/User/{shortUID}.json?auth={idToken}",
                        Method = "GET",
                        EnableDebug = true
                    };

                    RestClient.Request(request, (err, res) =>
                    {
                        if (err != null)
                        {
                            Debug.LogError($"데이터를 가져오는 도중 오류 발생 " + err.Message);
                            return;
                        }

                        var snapshot = JObject.Parse(res.Text);
                        if (snapshot != null)
                        {
                            string json = snapshot["jsonData"]?.ToString();
                            User loadUser = JsonUtility.FromJson<User>(json);
                            gameData.users[userId] = loadUser;
                        }
                    });
                }
                else
                {
                    Debug.Log("토큰 갱신 실패");
                }
            });

            yield return null;
        }

        public void ReadUserData(string shortUID, Action callback = null)
        {
            RestAPIAuth.Instance.EnsureIdToken(success =>
            {
                if (success)
                {
                    string idToken = RestAPIAuth.Instance.IdToken;

                    var request = new RequestHelper
                    {
                        Uri = $"{RestAPIAuth.Instance.dbUrl}/User/{shortUID}.json?auth={idToken}",
                        Method = "GET",
                        EnableDebug = true
                    };

                    RestClient.Request(request, (err, res) =>
                    {
                        if (err != null)
                        {
                            Debug.LogError($"데이터를 읽어오는데 실패했습니다.");
                            return;
                        }

                        var snapshot = JObject.Parse(res.Text);
                        if (snapshot != null)
                        {
                            foreach (var child in snapshot.Children())
                            {
                                string strData = child.First["jsonData"].ToString();
                                User loadUser = JsonUtility.FromJson<User>(strData);
                                gameData.users[shortUID] = loadUser;
                                Debug.Log("gameData : " + gameData.users[shortUID]);
                            }
                        }

                        callback?.Invoke();
                    });
                }
                else
                {
                    Debug.Log("토큰 갱신 실패 2");
                }
            });
        }

        public void WriteUserData(string shortUID, string jsonData = "", string customJsonData = "")
        {
            RestAPIAuth.Instance.EnsureIdToken(success =>
            {
                if (success)
                {
                    string idToken = RestAPIAuth.Instance.IdToken;

                    var getRequest = new RequestHelper
                    {
                        Uri = $"{RestAPIAuth.Instance.dbUrl}/User/{shortUID}.json?auth={idToken}",
                        Method = "GET",
                        EnableDebug = true
                    };

                    RestClient.Request(getRequest, (getErr, getRes) =>
                    {
                        if (getErr != null)
                        {
                            Debug.LogError($"데이터 업데이트 도중 오류 발생 : {getErr.Message}");
                            return;
                        }

                        JObject existingData = getRes.Text != null ? JObject.Parse(getRes.Text) : new JObject();

                        if (!string.IsNullOrEmpty(jsonData))
                        {
                            existingData["jsonData"] = JObject.Parse(jsonData);
                        }

                        if (!string.IsNullOrEmpty(customJsonData))
                        {
                            existingData["customData"] = JObject.Parse(customJsonData);
                        }

                        var putRequest = new RequestHelper
                        {
                            Uri = $"{RestAPIAuth.Instance.dbUrl}/User/{shortUID}.json?auth={idToken}",
                            Method = "PUT",
                            BodyString = existingData.ToString(),
                            EnableDebug = true
                        };

                        RestClient.Request(putRequest, (putErr, putRes) =>
                        {
                            if (putErr != null)
                            {
                                Debug.LogError($"데이터 업데이트 도중 오류 발생 : {putErr.Message}");
                            }
                            else
                            {
                                Debug.Log("데이터 업데이트 성공");
                            }
                        });
                    });
                }
                else
                {
                    Debug.LogError("토큰 갱신 실패 3");
                }
            });
        }

        public IEnumerator SignOutProcess(string shortUID, string jsonData, string customJsonData = "")
        {
            RestAPIAuth.Instance.EnsureIdToken(success =>
            {
                if (success)
                {
                    string idToken = RestAPIAuth.Instance.IdToken;

                    var getRequest = new RequestHelper
                    {
                        Uri = $"{RestAPIAuth.Instance.dbUrl}/User/{shortUID}.json?auth={idToken}",
                        Method = "GET",
                        EnableDebug = true
                    };

                    RestClient.Request(getRequest, (getErr, getRes) =>
                    {
                        if (getErr != null)
                        {
                            Debug.LogError($"기존 데이터를 가져오는 도중 오류 발생 : {getErr.Message}");
                            return;
                        }

                        JObject existingData = JObject.Parse(getRes.Text);

                        if (!string.IsNullOrEmpty(jsonData))
                        {
                            existingData["jsonData"] = JObject.Parse(jsonData);
                        }

                        if (!string.IsNullOrEmpty(customJsonData))
                        {
                            existingData["customData"] = JObject.Parse(customJsonData);
                        }
                        else if (existingData["customData"] == null)
                        {
                            PlayerCharacterCustomized playerCharacterCustomized = new PlayerCharacterCustomized();
                            string customData = playerCharacterCustomized.Initialize();
                            existingData["customData"] = JToken.Parse(customData);
                        }

                        var putRequest = new RequestHelper
                        {
                            Uri = $"{RestAPIAuth.Instance.dbUrl}/User/{shortUID}.json?auth={idToken}",
                            Method = "PUT",
                            BodyString = existingData.ToString(),
                            EnableDebug = true
                        };

                        RestClient.Request(putRequest, (putErr, putRes) =>
                        {
                            if (putErr != null)
                            {
                                Debug.LogError($"데이터 업데이트 도중 오류 발생 : {putErr.Message}");
                                return;
                            }

                            Debug.Log("업데이트 성공");

                            PerformLogOut();
                        });
                    });
                }
            });

            yield return null;

            //게임나가기
            Application.Quit();
        }

        private void PerformLogOut()
        {
            RestAPIAuth.Instance.ClearToken();
            Debug.Log("로그아웃 성공");
        }

        //public string GetUserJsonData(FirebaseUser user)
        //{
        //    User userData = gameData.users[user.UserId];
        //    string jsonData = JsonUtility.ToJson(userData);
        //    return jsonData;
        //}
    }
}

