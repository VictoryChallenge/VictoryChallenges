using UnityEngine;
using Photon.Pun;
using Proyecto26;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using TMPro;
using VictoryChallenge.KJ.Photon;
using VictoryChallenge.KJ.Database;
using VictoryChallenge.Customize;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace VictoryChallenge.KJ.Auth
{
    public class RestAPIAuth : SingletonLazy<RestAPIAuth>
    {
        /* ���̾�̽� Rest API */
        [HideInInspector] public string apiKey = "AIzaSyDklGzIx5gYLyPYJm_8I3yo8FJnMTOokIA";
        [HideInInspector] public string dbUrl = "https://victorychallenge-b8854-default-rtdb.firebaseio.com";
        private string _idToken;
        private string _userId;
        private string _displayName;
        private string _refreshToken;
        private DateTime _tokenExpiryTime; 

        public string IdToken => _idToken;
        public string UserId => _userId;
        public string DisplayName => _displayName;

        [Header("Log-In")]
        [Tooltip("�α��ο� �ʿ��� UI")]
        [HideInInspector] public TMP_InputField email;                                
        [HideInInspector] public TMP_InputField password;                             
        [HideInInspector] public TMP_Text warningLoginText;                          
        [HideInInspector] public TMP_Text confirmLoginText;                           

        [Header("Register")]
        [Tooltip("ȸ�����Կ� �ʿ��� UI")]
        [HideInInspector] public TMP_InputField usernameRegister;                     
        [HideInInspector] public TMP_InputField emailRegister;                        
        [HideInInspector] public TMP_InputField passwordRegister;                     
        [HideInInspector] public TMP_InputField passwordCheck;                       
        [HideInInspector] public TMP_Text warningRegisterText;                        
        [HideInInspector] public TMP_Text confirmRegisterText;

        #region �α���
        public void LoginButton(Action<bool> onLoginCompleted)
        {
            StartCoroutine(Login(email.text, password.text, onLoginCompleted));
        }

        public void AttemptLogin()
        {
            LoginButton(Result =>
            {
                if (Result)
                {
                    PhotonNetwork.LoadLevel("MainSceneCL(T)");
                    PhotonManager.Instance.CheckNetwork();
                }
                else
                {
                    Debug.Log("�α��� ����");
                }
            });
        }

        IEnumerator Login(string _email, string _password, Action<bool> onLoginCompleted)
        {
            var payload = new
            {
                email = _email,
                password = _password,
                returnSecureToken = true
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);

            var request = new RequestHelper
            {
                Uri = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}",
                Method = "POST",
                BodyString = jsonPayload,
                EnableDebug = true
            };

            RestClient.Request(request, (err, res) =>
            {
                if (err != null)
                {
                    Debug.LogError($"�α��� ���� : {err.Message}");
                    string message = "�α��ο� �����߽��ϴ�.";
                    
                    if (res != null)
                    {
                        var errorResponse = JObject.Parse(res.Text);
                        var error = errorResponse["error"];

                        if (error != null)
                        {
                            var errorCode = error["message"].ToString();

                            switch(errorCode)
                            {
                                case "EMAIL_NOT_FOUND":
                                    message = "�̸����� ã�� �� �����ϴ�.";
                                    break;
                                case "INVALID_PASSWORD":
                                    message = "�߸��� ��й�ȣ�Դϴ�.";
                                    break;
                                case "USER_DISABLED":
                                    message = "����� ������ ��Ȱ��ȭ�Ǿ����ϴ�.";
                                    break;
                                case "INVALID_EMAIL":
                                    message = "�߸��� �̸����Դϴ�. �ٽ� Ȯ�����ּ���.";
                                    break;
                                case "MISSING_PASSWORD":
                                    message = "��й�ȣ�� �Է����ּ���.";
                                    break;
                                case "MISSING_EMAIL":
                                    message = "�̸����� �Է����ּ���";
                                    break;
                                case "EMAIL_EXISTS":
                                    message = "�� �̸����� �̹� ��� ���Դϴ�.";
                                    break;
                                case "NETWORK_REQUEST_FAILED":
                                    message = "��Ʈ��ũ �����Դϴ�. ��Ʈ��ũ ������ Ȯ���ϼ���.";
                                    break;
                                default:
                                    message = $"���� �߻�: {errorCode}";
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (res.StatusCode)
                        {
                            case 400:
                                message = "�߸��� ��û�Դϴ�. �̸��ϰ� ��й�ȣ�� Ȯ���ϼ���.";
                                break;
                            case 401:
                                message = "������ �����߽��ϴ�. �̸��ϰ� ��й�ȣ�� Ȯ���ϼ���.";
                                break;
                            case 404:
                                message = "����ڸ� ã�� �� �����ϴ�.";
                                break;
                            case 500:
                                message = "���� �����Դϴ�. ��� �� �ٽ� �õ��ϼ���.";
                                break;
                            default:
                                message = $"���� �߻�: {res.StatusCode}";
                                break;
                        }
                    }

                    warningLoginText.text = message;
                    onLoginCompleted?.Invoke(false);
                    return;
                }

                var json = JObject.Parse(res.Text);
                _idToken = json["idToken"].ToString();
                _userId = json["localId"].ToString();
                _refreshToken = json["refreshToken"].ToString();
                _tokenExpiryTime = DateTime.Now.AddSeconds(Convert.ToDouble(json["expiresIn"].ToString()));

                Debug.Log($"�α��� ���� : {json["email"]}, {json["password"]}, {json["displayName"]}");

                StartCoroutine(FetchUserData(onLoginCompleted));
            });
            yield return null;
        }

        IEnumerator FetchUserData(Action<bool> onLoginCompleted)
        {
            var request = new RequestHelper
            {
                Uri = $"{dbUrl}/User/{_userId}.json?auth={_idToken}",
                Method = "GET",
                EnableDebug = true
            };

            RestClient.Request(request, (err, res) =>
            {
                if (err != null)
                {
                    Debug.LogError($"������ �ҷ����� ���� ���� �߻� : {err.Message}");
                    onLoginCompleted?.Invoke(false);
                    return;
                }

                var snapshot = JObject.Parse(res.Text);
                if (snapshot != null)
                {
                    string customData = snapshot["customData"]?.ToString();
                    string userJsonData = snapshot["jsonData"]?.ToString();

                    JObject authJsonData = JObject.Parse(userJsonData);
                    bool isLoggedIn = authJsonData["isLoggedIn"].Value<bool>();
                    Debug.Log("������ : " + isLoggedIn);

                    User userData = JsonUtility.FromJson<User>(userJsonData);

                    if (userData.isLoggedIn)
                    {
                        warningLoginText.text = " �̹� �������� ���̵� �Դϴ�.";
                        onLoginCompleted?.Invoke(false);
                        return;
                    }

                    userData.uid = _userId;
                    userData.shortUID = UIDHelper.GenerateShortUID(_userId);
                    userData.userName = authJsonData["displayName"].ToString();
                    userData.isLoggedIn = true;

                    Debug.Log("�÷��̾� ���� ���� : " + userData.isLoggedIn);

                    string updateJsonData = JsonUtility.ToJson(userData);

                    StartCoroutine(UpdateUserData(userData.shortUID, updateJsonData, authJsonData.ToString(), onLoginCompleted));
                }
                else
                {
                    Debug.LogError("����� �����͸� ã�� �� �����ϴ�.");
                    onLoginCompleted?.Invoke(false);
                }
            });
            yield return null;
        }

        IEnumerator UpdateUserData(string shortUID, string updateJsonData, string authJsonData, Action<bool> onLoginCompleted)
        {
            var getRequest = new RequestHelper
            {
                Uri = $"{dbUrl}/User/{shortUID}.json?auth={_idToken}",
                Method = "GET",
                EnableDebug = true,
            };

            RestClient.Request(getRequest, (getErr, getRes) =>
            {
                if (getErr != null)
                {
                    Debug.LogError($"������ ������Ʈ ���� ���� �߻� : {getErr.Message}");
                    onLoginCompleted?.Invoke(false);
                    return;
                }

                var existingData = JObject.Parse(getRes.Text);

                var updateData = JObject.Parse(updateJsonData);
                existingData["jsonData"] = updateData["jsonData"];
                if (updateData.ContainsKey("customData"))
                {
                    existingData["customData"] = updateData["customData"];
                }

                var putRequest = new RequestHelper
                {
                    Uri = $"{dbUrl}/User/{shortUID}.json?auth={_idToken}",
                    Method = "PUT",
                    BodyString = existingData.ToString(),
                    EnableDebug = true
                };

                RestClient.Request(putRequest, (putErr, putRes) =>
                {
                    if (putErr != null)
                    {
                        Debug.LogError($"������ ������Ʈ �� ���� �߻� : {putErr.Message}");
                        onLoginCompleted?.Invoke(false);
                        return;
                    }
                });

                var disconnectRequest = new RequestHelper
                {
                    Uri = $"{dbUrl}/User/{shortUID}/jsonData.json?auth={_idToken}",
                    Method = "PUT",
                    BodyString = authJsonData.Replace("\"isLoggedIn\":true", "\"isLoggedIn\":false"),
                    EnableDebug = true,
                };

                RestClient.Request(disconnectRequest, (disconnectErr, disconnectRes) =>
                {
                    if (disconnectErr != null)
                    {
                        Debug.LogError($"OnDisconnect ������ ���� �߻� : {disconnectErr.Message}");
                        onLoginCompleted?.Invoke(false);
                        return;
                    }

                    warningLoginText.text = " ";
                    confirmLoginText.text = "�α��ο� �����߽��ϴ�.";
                    onLoginCompleted?.Invoke(true);
                });
            });

            yield return null;
        }

        private IEnumerator RefreshIdToken(Action<bool> onTokenRefreshed)
        {
            var refreshPayload = new
            {
                grant_type = "refresh_token",
                refresh_token = _refreshToken
            };

            var jsonRefreshPayload = JsonConvert.SerializeObject(refreshPayload);

            var request = new RequestHelper
            {
                Uri = $"https://securetoken.googleapis.com/v1/token?key={apiKey}",
                Method = "POST",
                BodyString = jsonRefreshPayload,
                EnableDebug = true
            };

            RestClient.Request(request, (err, res) =>
            {
                if (err != null)
                {
                    Debug.LogError($"��ū ���� ���� {err.Message}");
                    onTokenRefreshed?.Invoke(false);
                    return;
                }

                var json = JObject.Parse(res.Text);
                _idToken = json["id_token"].ToString();
                _refreshToken = json["refresh_token"].ToString();
                _tokenExpiryTime = DateTime.Now.AddSeconds(Convert.ToDouble(json["expires_in"].ToString()));

                onTokenRefreshed?.Invoke(true);
            });
            yield return null;
        }

        public void EnsureIdToken(Action<bool> onTokenEnsured)
        {
            if (IsTokenExpired())
            {
                StartCoroutine(RefreshIdToken(onTokenEnsured));
            }
            else
            {
                onTokenEnsured?.Invoke(true);
            }
        }

        private bool IsTokenExpired()
        {
            return DateTime.Now >= _tokenExpiryTime;
        }

        public void ClearToken()
        {
            _idToken = null;
            _refreshToken = null;
            _userId = null;
            _tokenExpiryTime = DateTime.MinValue;
        }
        #endregion

        #region �α׾ƿ�
        public void LogOut()
        {
            if (_userId != null)
            {
                string shortUID = UIDHelper.GenerateShortUID(_userId);
                
                if (DBTutorial.Instance.gameData.users.ContainsKey(shortUID))
                {
                    DBTutorial.Instance.gameData.users[shortUID].isLoggedIn = false;
                    User userData = DBTutorial.Instance.gameData.users[shortUID];
                    string json = JsonUtility.ToJson(userData);
                    string customData = DBTutorial.Instance.customData;

                    StartCoroutine(DBTutorial.Instance.SignOutProcess(shortUID, json, customData));
                    Debug.Log("���� OFF : " + DBTutorial.Instance.gameData.users[shortUID].isLoggedIn);
                }
                else
                {
                    Debug.LogError("����� �����͸� ã�� �� �����ϴ�.");
                }
            }
            else
            {
                Debug.Log("�α��ε� ������ �����ϴ�.");
            }
        }
        #endregion

        #region ȸ������
        public void RegisterButton()
        {
            StartCoroutine(Register(emailRegister.text, passwordRegister.text, usernameRegister.text));
        }

        public void SetDisplayName(string displayName)
        {
            _displayName = displayName;
        }

        IEnumerator Register(string _email, string _password, string _username)
        {
            if (string.IsNullOrWhiteSpace(_username))
            {
                warningRegisterText.text = "�г����� �Է����ּ���.";
            }
            else if (passwordRegister.text != passwordCheck.text)
            {
                warningRegisterText.text = "��й�ȣ�� ��ġ���� �ʽ��ϴ�.";
            }
            else
            {
                var payload = new
                {
                    email = _email,
                    password = _password,
                    returnSecureToken = true
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);

                var request = new RequestHelper
                {
                    Uri = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}",
                    Method = "POST",
                    BodyString = jsonPayload,
                    EnableDebug = true,
                    Headers = new Dictionary<string, string>
                    {
                        {"Content-Type", "application/json" }
                    }
                };

                bool isRequestCompleted = false;
                RestClient.Request(request, (err, res) =>
                {
                    if (err != null)
                    {
                        Debug.LogError($"ȸ������ �� ���� �߻� : {err.Message}");
                        string message = "ȸ�����Կ� �����Ͽ����ϴ�.";

                        if (res != null)
                        {
                            var errorResponse = JObject.Parse(res.Text);
                            var error = errorResponse["error"];
                            Debug.LogError("���� ���� �ڵ� : " + res.StatusCode);
                            Debug.LogError("���� ���� : " + res.Text);
                            Debug.Log($" Ȯ�� �ʿ� :  {payload}, {request.Uri}, {request.Body}, {request.Method}");

                            if (error != null)
                            {
                                var errorCode = error["message"].ToString();

                                switch (errorCode)
                                {
                                    case "EMAIL_EXISTS":
                                        message = "�̸����� �̹� ������Դϴ�.";
                                        break;
                                    case "INVALID_PASSWORD":
                                        message = "�߸��� ��й�ȣ�Դϴ�.";
                                        break;
                                    case "NETWORK_REQUEST_FAILED":
                                        message = "��Ʈ��ũ �����Դϴ�. ��Ʈ��ũ ������ Ȯ���ϼ���.";
                                        break;
                                    default:
                                        message = $"���� �߻�: {errorCode}";
                                        break;
                                }
                            }
                            else
                            {
                                switch (res.StatusCode)
                                {
                                    case 400:
                                        message = "�߸��� ��û�Դϴ�. �̸��ϰ� ��й�ȣ�� Ȯ���ϼ���.";
                                        break;
                                    case 409:
                                        message = "�̸����� �̹� ��� ���Դϴ�.";
                                        break;
                                    case 500:
                                        message = "���� �����Դϴ�. ��� �� �ٽ� �õ��ϼ���.";
                                        break;
                                    default:
                                        message = $"���� �߻�: {res.StatusCode}";
                                        break;
                                }
                            }
                        }

                        warningRegisterText.text = message;
                        isRequestCompleted = true;
                        return;
                    }

                    var json = JObject.Parse(res.Text);
                    _idToken = json["idToken"].ToString();
                    _refreshToken = json["refreshToken"].ToString();
                    _userId = json["localId"].ToString();
                    SetDisplayName(_username);

                    var profilePayload = new
                    {
                        idToken = _idToken,
                        displayName = _username,
                        returnSecureToken = true
                    };

                    var jsonProfilePaylod = JsonConvert.SerializeObject(profilePayload);

                    Debug.Log("������ ������Ʈ ��û payload: " + JsonUtility.ToJson(profilePayload));

                    var profileRequest = new RequestHelper
                    {
                        Uri = $"https://identitytoolkit.googleapis.com/v1/accounts:update?key={apiKey}",
                        Method = "POST",
                        BodyString = jsonProfilePaylod,
                        EnableDebug = true
                    };

                    RestClient.Request(profileRequest, (profileErr, profileRes) =>
                    {
                        if (profileErr != null)
                        {
                            Debug.LogError($"������ ������Ʈ �� ���� �߻� : {profileErr.Message}");
                            warningRegisterText.text = "������ ������Ʈ �� ���� �߻�";
                        }
                        else
                        {
                            PlayerCharacterCustomized playerData = new PlayerCharacterCustomized();
                            string customData = playerData.Initialize();
                            DBTutorial.Instance.customData = customData;

                            string shortUID = UIDHelper.GenerateShortUID(_userId);
                            User newUser = new User(_userId, shortUID, _username, false, 100, 0);
                            string jsonData = JsonUtility.ToJson(newUser);
                            DBTutorial.Instance.userData = jsonData;

                            DBTutorial.Instance.WriteUserData(shortUID, jsonData, customData);

                            Debug.Log("ȸ�������� ���������� �̷�������ϴ�." + newUser.userName);
                            confirmRegisterText.text = "ȸ�������� ���������� �̷�������ϴ�.";
                            warningRegisterText.text = "";
                        }
                        isRequestCompleted = true;
                    });
                });

                while (!isRequestCompleted)
                {
                    yield return null;
                }
            }
        }
        #endregion
    }
}
