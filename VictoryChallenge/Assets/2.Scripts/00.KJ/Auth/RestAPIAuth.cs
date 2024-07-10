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
        /* 파이어베이스 Rest API */
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
        [Tooltip("로그인에 필요한 UI")]
        [HideInInspector] public TMP_InputField email;                                
        [HideInInspector] public TMP_InputField password;                             
        [HideInInspector] public TMP_Text warningLoginText;                          
        [HideInInspector] public TMP_Text confirmLoginText;                           

        [Header("Register")]
        [Tooltip("회원가입에 필요한 UI")]
        [HideInInspector] public TMP_InputField usernameRegister;                     
        [HideInInspector] public TMP_InputField emailRegister;                        
        [HideInInspector] public TMP_InputField passwordRegister;                     
        [HideInInspector] public TMP_InputField passwordCheck;                       
        [HideInInspector] public TMP_Text warningRegisterText;                        
        [HideInInspector] public TMP_Text confirmRegisterText;

        #region 로그인
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
                    Debug.Log("로그인 실패");
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
                    Debug.LogError($"로그인 실패 : {err.Message}");
                    string message = "로그인에 실패했습니다.";
                    
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
                                    message = "이메일을 찾을 수 없습니다.";
                                    break;
                                case "INVALID_PASSWORD":
                                    message = "잘못된 비밀번호입니다.";
                                    break;
                                case "USER_DISABLED":
                                    message = "사용자 계정이 비활성화되었습니다.";
                                    break;
                                case "INVALID_EMAIL":
                                    message = "잘못된 이메일입니다. 다시 확인해주세요.";
                                    break;
                                case "MISSING_PASSWORD":
                                    message = "비밀번호를 입력해주세요.";
                                    break;
                                case "MISSING_EMAIL":
                                    message = "이메일을 입력해주세요";
                                    break;
                                case "EMAIL_EXISTS":
                                    message = "이 이메일은 이미 사용 중입니다.";
                                    break;
                                case "NETWORK_REQUEST_FAILED":
                                    message = "네트워크 오류입니다. 네트워크 연결을 확인하세요.";
                                    break;
                                default:
                                    message = $"오류 발생: {errorCode}";
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (res.StatusCode)
                        {
                            case 400:
                                message = "잘못된 요청입니다. 이메일과 비밀번호를 확인하세요.";
                                break;
                            case 401:
                                message = "인증에 실패했습니다. 이메일과 비밀번호를 확인하세요.";
                                break;
                            case 404:
                                message = "사용자를 찾을 수 없습니다.";
                                break;
                            case 500:
                                message = "서버 오류입니다. 잠시 후 다시 시도하세요.";
                                break;
                            default:
                                message = $"오류 발생: {res.StatusCode}";
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

                Debug.Log($"로그인 성공 : {json["email"]}, {json["password"]}, {json["displayName"]}");

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
                    Debug.LogError($"데이터 불러오는 도중 오류 발생 : {err.Message}");
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
                    Debug.Log("접속중 : " + isLoggedIn);

                    User userData = JsonUtility.FromJson<User>(userJsonData);

                    if (userData.isLoggedIn)
                    {
                        warningLoginText.text = " 이미 접속중인 아이디 입니다.";
                        onLoginCompleted?.Invoke(false);
                        return;
                    }

                    userData.uid = _userId;
                    userData.shortUID = UIDHelper.GenerateShortUID(_userId);
                    userData.userName = authJsonData["displayName"].ToString();
                    userData.isLoggedIn = true;

                    Debug.Log("플레이어 접속 상태 : " + userData.isLoggedIn);

                    string updateJsonData = JsonUtility.ToJson(userData);

                    StartCoroutine(UpdateUserData(userData.shortUID, updateJsonData, authJsonData.ToString(), onLoginCompleted));
                }
                else
                {
                    Debug.LogError("사용자 데이터를 찾을 수 없습니다.");
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
                    Debug.LogError($"데이터 업데이트 도중 문제 발생 : {getErr.Message}");
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
                        Debug.LogError($"데이터 업데이트 중 오류 발생 : {putErr.Message}");
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
                        Debug.LogError($"OnDisconnect 설정중 오류 발생 : {disconnectErr.Message}");
                        onLoginCompleted?.Invoke(false);
                        return;
                    }

                    warningLoginText.text = " ";
                    confirmLoginText.text = "로그인에 성공했습니다.";
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
                    Debug.LogError($"토큰 갱신 실패 {err.Message}");
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

        #region 로그아웃
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
                    Debug.Log("접속 OFF : " + DBTutorial.Instance.gameData.users[shortUID].isLoggedIn);
                }
                else
                {
                    Debug.LogError("사용자 데이터를 찾을 수 없습니다.");
                }
            }
            else
            {
                Debug.Log("로그인된 계정이 없습니다.");
            }
        }
        #endregion

        #region 회원가입
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
                warningRegisterText.text = "닉네임을 입력해주세요.";
            }
            else if (passwordRegister.text != passwordCheck.text)
            {
                warningRegisterText.text = "비밀번호가 일치하지 않습니다.";
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
                        Debug.LogError($"회원가입 중 오류 발생 : {err.Message}");
                        string message = "회원가입에 실패하였습니다.";

                        if (res != null)
                        {
                            var errorResponse = JObject.Parse(res.Text);
                            var error = errorResponse["error"];
                            Debug.LogError("응답 상태 코드 : " + res.StatusCode);
                            Debug.LogError("응답 내용 : " + res.Text);
                            Debug.Log($" 확인 필요 :  {payload}, {request.Uri}, {request.Body}, {request.Method}");

                            if (error != null)
                            {
                                var errorCode = error["message"].ToString();

                                switch (errorCode)
                                {
                                    case "EMAIL_EXISTS":
                                        message = "이메일이 이미 사용중입니다.";
                                        break;
                                    case "INVALID_PASSWORD":
                                        message = "잘못된 비밀번호입니다.";
                                        break;
                                    case "NETWORK_REQUEST_FAILED":
                                        message = "네트워크 오류입니다. 네트워크 연결을 확인하세요.";
                                        break;
                                    default:
                                        message = $"오류 발생: {errorCode}";
                                        break;
                                }
                            }
                            else
                            {
                                switch (res.StatusCode)
                                {
                                    case 400:
                                        message = "잘못된 요청입니다. 이메일과 비밀번호를 확인하세요.";
                                        break;
                                    case 409:
                                        message = "이메일이 이미 사용 중입니다.";
                                        break;
                                    case 500:
                                        message = "서버 오류입니다. 잠시 후 다시 시도하세요.";
                                        break;
                                    default:
                                        message = $"오류 발생: {res.StatusCode}";
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

                    Debug.Log("프로필 업데이트 요청 payload: " + JsonUtility.ToJson(profilePayload));

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
                            Debug.LogError($"프로필 업데이트 중 오류 발생 : {profileErr.Message}");
                            warningRegisterText.text = "프로필 업데이트 중 오류 발생";
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

                            Debug.Log("회원가입이 성공적으로 이루어졌습니다." + newUser.userName);
                            confirmRegisterText.text = "회원가입이 성공적으로 이루어졌습니다.";
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
