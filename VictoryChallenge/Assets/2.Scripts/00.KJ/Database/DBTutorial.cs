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
                Debug.Log("gameData �ʱ�ȭ");
            }
        }

        public void SaveUserDB(string userId)
        {
            if (gameData == null || !gameData.users.ContainsKey(userId))
            {
                Debug.LogError("����� �����Ͱ� �ʱ�ȭ ���� �ʾҰų� �����Ͱ� �����ϴ�.");
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
                            Debug.LogError($"�����͸� �������� ���� ���� �߻� " + err.Message);
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
                    Debug.Log("��ū ���� ����");
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
                            Debug.LogError($"�����͸� �о���µ� �����߽��ϴ�.");
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
                    Debug.Log("��ū ���� ���� 2");
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
                            Debug.LogError($"������ ������Ʈ ���� ���� �߻� : {getErr.Message}");
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
                                Debug.LogError($"������ ������Ʈ ���� ���� �߻� : {putErr.Message}");
                            }
                            else
                            {
                                Debug.Log("������ ������Ʈ ����");
                            }
                        });
                    });
                }
                else
                {
                    Debug.LogError("��ū ���� ���� 3");
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
                            Debug.LogError($"���� �����͸� �������� ���� ���� �߻� : {getErr.Message}");
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
                                Debug.LogError($"������ ������Ʈ ���� ���� �߻� : {putErr.Message}");
                                return;
                            }

                            Debug.Log("������Ʈ ����");

                            PerformLogOut();
                        });
                    });
                }
            });

            yield return null;

            //���ӳ�����
            Application.Quit();
        }

        private void PerformLogOut()
        {
            RestAPIAuth.Instance.ClearToken();
            Debug.Log("�α׾ƿ� ����");
        }

        //public string GetUserJsonData(FirebaseUser user)
        //{
        //    User userData = gameData.users[user.UserId];
        //    string jsonData = JsonUtility.ToJson(userData);
        //    return jsonData;
        //}
    }
}

