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
using QFSW.QC.Actions;


namespace VictoryChallenge.KJ.Auth
{
    /// <summary>
    /// Firebase REST API�� ���� ����� ���� ��� ����
    /// </summary>
    public class RestAPIAuth : SingletonLazy<RestAPIAuth>
    {
        /* ���̾�̽� Rest API */
        [HideInInspector] public string apiKey = "AIzaSyDklGzIx5gYLyPYJm_8I3yo8FJnMTOokIA";                     // Firebase Project Key
        [HideInInspector] public string dbUrl = "https://victorychallenge-b8854-default-rtdb.firebaseio.com";   // Firebase RealtimeDatabase URL
        private string _idToken;            // Firebase ����� ��ū
        private string _userId;             // Firebase ���� ���̵�
        private string _displayName;        // Firebase ���� �г��� 
        private string _refreshToken;       // FIrebaes ��߱� ��ū
        private DateTime _tokenExpiryTime;  // FIrebase ��ū ���� ���� �ð�

        public string IdToken => _idToken;
        public string UserId => _userId;
        public string DisplayName => _displayName;

        [Header("Log-In")]
        [Tooltip("�α��ο� �ʿ��� UI")]
        [HideInInspector] public TMP_InputField email;              // �α��� �̸��� �Է� �ʵ�                     
        [HideInInspector] public TMP_InputField password;           // �α��� ��й�ȣ �Է� �ʵ�                     
        [HideInInspector] public TMP_Text warningLoginText;         // �α��� ���� �޼��� ǥ��                 
        [HideInInspector] public TMP_Text confirmLoginText;         // �α��� ���� �޼��� ǥ��                  

        [Header("Register")]
        [Tooltip("ȸ�����Կ� �ʿ��� UI")]
        [HideInInspector] public TMP_InputField usernameRegister;   // ȸ������ �г��� �Է� �ʵ�                  
        [HideInInspector] public TMP_InputField emailRegister;      // ȸ������ �̸��� �Է� �ʵ�                  
        [HideInInspector] public TMP_InputField passwordRegister;   // ȸ������ ��й�ȣ �Է� �ʵ�                  
        [HideInInspector] public TMP_InputField passwordCheck;      // ��й�ȣ üũ                 
        [HideInInspector] public TMP_Text warningRegisterText;      // ȸ������ ���� �޼��� ǥ��                  
        [HideInInspector] public TMP_Text confirmRegisterText;      // ȸ������ ���� �޼��� ǥ��

        #region �α���
        /// <summary>
        /// �α��� ��ư
        /// </summary>
        /// <param name="onLoginCompleted"> �ݹ� �޼��� ���� </param>
        public void LoginButton(Action<bool> onLoginCompleted)
        {
            // �α��� �ڷ�ƾ ����
            StartCoroutine(Login(email.text, password.text, onLoginCompleted));
        }

        /// <summary>
        /// �α��� �õ�
        /// </summary>
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

        /// <summary>
        /// Firebase�� �̸��ϰ� ��й�ȣ�� ������ �α����� �õ�
        /// </summary>
        /// <param name="_email"> Firebase�� ���� �̸��� </param>
        /// <param name="_password"> Firebase�� ���� ��й�ȣ </param>
        /// <param name="onLoginCompleted"> �α��� �������ο� ���� ȣ��Ǵ� �ݹ� �޼��� <param>
        /// <returns></returns>
        IEnumerator Login(string _email, string _password, Action<bool> onLoginCompleted)
        {
            /* email, password, returnSecureToken ���� ������ �͸� ��ü�� ���� */
            var payload = new
            {
                email = _email,
                password = _password,
                returnSecureToken = true
            };

            /* Json ���ڿ��� ��ȯ */
            var jsonPayload = JsonConvert.SerializeObject(payload);

            /* ��û ���� */
            var request = new RequestHelper
            {
                /* Firebase ���� ��������Ʈ URL */ 
                Uri = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}",
                /* �α��� ��û */
                Method = "POST",
                /* ������ ���Ե� json */
                BodyString = jsonPayload,
                /* ����� ��� Ȱ��ȭ */
                EnableDebug = true
            };

            /* ��û ������ */
            RestClient.Request(request, (err, res) =>
            {
                /* ���� ó�� */
                if (err != null)
                {
                    Debug.LogError($"�α��� ���� : {err.Message}");
                    string message = "�α��ο� �����߽��ϴ�.";
                    
                    /* ���� ���� ó�� */
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
                    /* ���� ���� ó�� */
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

                    /* ���� �޼��� UI�� ��� �� �ݹ� �޼��� ȣ�� */
                    warningLoginText.text = message;
                    onLoginCompleted?.Invoke(false);
                    return;
                }

                /* �α��� ���� ó��, Firebase���� ���� �� �������� ���� */
                var json = JObject.Parse(res.Text);
                _idToken = json["idToken"].ToString();
                _userId = json["localId"].ToString();
                _refreshToken = json["refreshToken"].ToString();
                _tokenExpiryTime = DateTime.Now.AddSeconds(Convert.ToDouble(json["expiresIn"].ToString()));

                Debug.Log($"�α��� ���� : {json["email"]}, {json["password"]}, {json["displayName"]}");

                string shortUID = UIDHelper.GenerateShortUID(_userId);

                /* ����� ������ �������� */
                StartCoroutine(FetchUserData(onLoginCompleted, shortUID));
            });
            /* �ڷ�ƾ ���� */
            yield return null;
        }

        /// <summary>
        /// DB�� ����� ����� ������ �������� 
        /// </summary>
        /// <param name="onLoginCompleted"> �α��� �Ϸ� ���� ������ �ݹ� �޼��� </param>
        /// <param name="shortUID"> ����� shortUID </param>
        /// <returns></returns>
        IEnumerator FetchUserData(Action<bool> onLoginCompleted, string shortUID)
        {
            /* ��û ���� */
            var request = new RequestHelper
            {
                /* �����ͺ��̽� URL, dbUrl + shortUID, _idToken �ʿ� */
                Uri = $"{dbUrl}/User/{shortUID}.json?auth={_idToken}",
                /* �����͸� ������ */
                Method = "GET",
                EnableDebug = true
            };

            /* ��û ������ */
            RestClient.Request(request, (err, res) =>
            {
                /* ���� ó�� 
                 * ��û �߿� ������ �߻��߰ų�
                   ���� �ؽ�Ʈ�� ����ְų�
                   ���信 "null"���� Ȯ�� */
                if (err != null || string.IsNullOrEmpty(res.Text) || res.Text == "null")
                {
                    onLoginCompleted?.Invoke(false);
                    return;
                }

                /* ���� ó�� */
                var snapshot = JObject.Parse(res.Text);
                if (snapshot != null)
                {
                    /* ����� customData, jsonData ���� */
                    string customData = snapshot["customData"]?.ToString();
                    string userJsonData = snapshot["jsonData"]?.ToString();

                    /* json �����͸� ���� ��ü�� ��ȯ */
                    User userData = JsonUtility.FromJson<User>(userJsonData);

                    /* �ߺ� �α��� Ȯ�� */
                    if (userData.isLoggedIn)
                    {
                        warningLoginText.text = " �̹� �������� ���̵� �Դϴ�.";
                        onLoginCompleted?.Invoke(false);
                        return;
                    }

                    /* ����� ������ ������Ʈ */
                    userData.uid = _userId;
                    userData.shortUID = UIDHelper.GenerateShortUID(_userId);
                    SetDisplayName(userData.userName);
                    userData.isLoggedIn = true;

                    Debug.Log("�÷��̾� ���� ���� : " + userData.isLoggedIn);

                    DBManager.Instance.WriteUserData(shortUID, userJsonData, customData);

                    /* ����� ������ ������Ʈ �ڷ�ƾ ȣ�� */
                    StartCoroutine(UpdateUserData(userData.shortUID, customData, userJsonData, onLoginCompleted));
                }
                else
                {
                    Debug.LogError("����� �����͸� ã�� �� �����ϴ�.");
                    onLoginCompleted?.Invoke(false);
                }
            });
            yield return null;
        }

        /// <summary>
        /// ������� �����͸� ������Ʈ
        /// </summary>
        /// <param name="shortUID"> ����� shortUID </param>
        /// <param name="customJsonData"> ������� Customizing </param>
        /// <param name="authJsonData"> ������� ���� ������ </param>
        /// <param name="onLoginCompleted"> �α��� �Ϸ� ���θ� �ݹ� �޼���� ���� </param>
        /// <returns></returns>
        IEnumerator UpdateUserData(string shortUID, string customJsonData, string authJsonData, Action<bool> onLoginCompleted)
        {
            /* ��û ���� */
            var getRequest = new RequestHelper
            {
                Uri = $"{dbUrl}/User/{shortUID}.json?auth={_idToken}",
                Method = "GET",
                EnableDebug = true,
            };

            /* ��û ������ */
            RestClient.Request(getRequest, (getErr, getRes) =>
            {
                /* ���� ó�� */
                if (getErr != null)
                {
                    Debug.LogError($"������ ������Ʈ ���� ���� �߻� : {getErr.Message}");
                    onLoginCompleted?.Invoke(false);
                    return;
                }

                /* ���� ������ �ʱ�ȭ */
                JObject existingData = new JObject();

                /* ���� ������ ó�� */
                if (!string.IsNullOrEmpty(authJsonData))                    // ���� �����Ͱ� ����ִ��� Ȯ��
                {
                    existingData["jsonData"] = authJsonData;                // ���� �����Ϳ� ���� ������ �߰�
                    JObject isLoggedInData = JObject.Parse(authJsonData);   // ���� �����͸� Json ��ü�� �Ľ�
                    if (isLoggedInData != null)
                    {
                        // �ߺ� �α��� ����
                        if(isLoggedInData["isLoggedIn"].Value<bool>() == true)
                        {
                            onLoginCompleted?.Invoke(false);
                            return;
                        }

                        isLoggedInData["isLoggedIn"] = true;
                        /* ������Ʈ �� ���� �����͸� json ���ڿ��� ��ȯ �� ���� */
                        existingData["jsonData"] = JsonConvert.SerializeObject(isLoggedInData); 
                    }
                }

                /* Customizing ������ ó�� */
                if (!string.IsNullOrEmpty(customJsonData))
                {
                    existingData["customData"] = customJsonData;
                }

                /* ������Ʈ ��û ���� */
                var putRequest = new RequestHelper
                {
                    Uri = $"{dbUrl}/User/{shortUID}.json?auth={_idToken}",
                    /* �����͸� ������Ʈ */
                    Method = "PUT",
                    BodyString = existingData.ToString(),
                    EnableDebug = true
                };

                /* ������Ʈ ��û ������ */
                RestClient.Request(putRequest, (putErr, putRes) =>
                {
                    /* ���� ó�� */
                    if (putErr != null)
                    {
                        Debug.LogError($"������ ������Ʈ �� ���� �߻� : {putErr.Message}");
                        onLoginCompleted?.Invoke(false);
                        return;
                    }
                });

                onLoginCompleted?.Invoke(true);
            });

            yield return null;
        }

        /// <summary>
        /// FIrebase ��ū�� ����Ǿ��� �� ���ο� ��ū �߱�
        /// </summary>
        /// <param name="onTokenRefreshed"> ��ū ���� ���θ� ������ �ݹ� �޼��� </param>
        /// <returns></returns>
        private IEnumerator RefreshIdToken(Action<bool> onTokenRefreshed)
        {
            /* payload ���� */
            var refreshPayload = new
            {
                grant_type = "refresh_token",   
                refresh_token = _refreshToken
            };

            /* payload ����ȭ */
            var jsonRefreshPayload = JsonConvert.SerializeObject(refreshPayload);

            /* ��û ���� */
            var request = new RequestHelper
            {
                Uri = $"https://securetoken.googleapis.com/v1/token?key={apiKey}",
                Method = "POST",
                BodyString = jsonRefreshPayload,
                EnableDebug = true
            };

            /* ��û ���� */
            RestClient.Request(request, (err, res) =>
            {
                /* ���� ó�� */
                if (err != null)
                {
                    Debug.LogError($"��ū ���� ���� {err.Message}");
                    onTokenRefreshed?.Invoke(false);
                    return;
                }

                /* ���� ó�� �� ��ū ���� */
                var json = JObject.Parse(res.Text);
                _idToken = json["id_token"].ToString();
                _refreshToken = json["refresh_token"].ToString();
                _tokenExpiryTime = DateTime.Now.AddSeconds(Convert.ToDouble(json["expires_in"].ToString()));

                onTokenRefreshed?.Invoke(true);
            });
            yield return null;
        }

        /// <summary>
        /// ��ū�� ����Ǿ����� Ȯ��
        /// </summary>
        /// <param name="onTokenEnsured"> ��ū ��ȿ�� �Ϸ� ���� ������ �ݹ� �޼��� </param>
        public void EnsureIdToken(Action<bool> onTokenEnsured)
        {
            if (IsTokenExpired())   // ��ū�� �Ϸ�Ǿ����� Ȯ��
            {
                StartCoroutine(RefreshIdToken(onTokenEnsured)); // ��ū ��߱�
            }
            else
            {
                onTokenEnsured?.Invoke(true);
            }
        }

        /// <summary>
        /// ��ū ���� ���� ��ȯ
        /// </summary>
        /// <returns></returns>
        private bool IsTokenExpired()
        {
            /* ��ū ���� �ð��� ������ true ��ȯ */
            return DateTime.Now >= _tokenExpiryTime;
        }

        /// <summary>
        ///  ����� ��ū ���� ����
        /// </summary>
        public void ClearToken()
        {
            _idToken = null;
            _refreshToken = null;
            _userId = null;
            _tokenExpiryTime = DateTime.MinValue;
        }
        #endregion

        #region �α׾ƿ�
        /// <summary>
        /// �α׾ƿ� �� �α׾ƿ��� ���� ����
        /// </summary>
        public void LogOut()
        {
            if (_userId != null)
            {
                string shortUID = UIDHelper.GenerateShortUID(_userId);
                
                if (DBManager.Instance.gameData.users.ContainsKey(shortUID))
                {
                    DBManager.Instance.gameData.users[shortUID].isLoggedIn = false;
                    User userData = DBManager.Instance.gameData.users[shortUID];
                    string json = JsonUtility.ToJson(userData);
                    string customData = DBManager.Instance.customData;

                    StartCoroutine(DBManager.Instance.SignOutProcess(shortUID, json, customData));
                    Debug.Log("���� OFF : " + DBManager.Instance.gameData.users[shortUID].isLoggedIn);
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
        /// <summary>
        /// ȸ�� ���� ��ư
        /// </summary>
        public void RegisterButton()
        {
            StartCoroutine(Register(emailRegister.text, passwordRegister.text, usernameRegister.text));
        }

        /// <summary>
        /// ���� �г��� ����
        /// </summary>
        /// <param name="displayName"> ���� �г��� </param>
        public void SetDisplayName(string displayName)
        {
            _displayName = displayName;
        }

        /// <summary>
        /// ȸ������ ����
        /// </summary>
        /// <param name="_email"> ȸ�����Կ� �ʿ��� �̸��� </param>
        /// <param name="_password"> ȸ�����Կ� �ʿ��� ��й�ȣ </param>
        /// <param name="_username"> ȸ�����Կ� �ʿ��� ���� �г��� </param>
        /// <returns></returns>
        IEnumerator Register(string _email, string _password, string _username)
        {
            /* ���� ������ ����� �� */
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
                /* ȸ������ payload */
                var payload = new
                {
                    email = _email,
                    password = _password,
                    returnSecureToken = true
                };

                /* ����ȭ */
                var jsonPayload = JsonConvert.SerializeObject(payload);

                /* ��û ���� */
                var request = new RequestHelper
                {
                    /* ȸ�������� ��û�� http */
                    Uri = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}",
                    Method = "POST",
                    BodyString = jsonPayload,
                    EnableDebug = true,
                    /* ��û �� ����� �Բ� ���۵Ǵ� ��Ÿ������, ��� ���� */
                    Headers = new Dictionary<string, string>
                    {
                        {"Content-Type", "application/json" }   // { ������ Ÿ��, ������ �Ľ� }
                    }
                };

                bool isRequestCompleted = false;    // ��û ���� false�� ����
                /* ��û ���� */
                RestClient.Request(request, (err, res) =>
                {
                    /* ���� ó�� */
                    if (err != null)
                    {
                        Debug.LogError($"ȸ������ �� ���� �߻� : {err.Message}");
                        string message = "ȸ�����Կ� �����Ͽ����ϴ�.";

                        /* ���� ���� ó�� */
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
                            /* ���� ���� ó�� */
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
                        isRequestCompleted = true;      // ��û ���� true
                        return;
                    }

                    /* ȸ������ ���� ���� */
                    var json = JObject.Parse(res.Text);
                    _idToken = json["idToken"].ToString();
                    _refreshToken = json["refreshToken"].ToString();
                    _userId = json["localId"].ToString();
                    SetDisplayName(_username);  // �г��� ����

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
                            /* ���� Ŀ���͸���¡ ������ ���� ���� ���� */
                            PlayerCharacterCustomized playerData = new PlayerCharacterCustomized();
                            string customData = playerData.Initialize();
                            DBManager.Instance.customData = customData;

                            string shortUID = UIDHelper.GenerateShortUID(_userId);
                            User newUser = new User(_userId, shortUID, _username, false, 100);
                            string jsonData = JsonUtility.ToJson(newUser);
                            DBManager.Instance.userData = jsonData;

                            DBManager.Instance.WriteUserData(shortUID, jsonData, customData);

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
