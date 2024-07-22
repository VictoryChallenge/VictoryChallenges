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
    /// Firebase REST API를 통해 사용자 관련 기능 제공
    /// </summary>
    public class RestAPIAuth : SingletonLazy<RestAPIAuth>
    {
        /* 파이어베이스 Rest API */
        [HideInInspector] public string apiKey = "AIzaSyDklGzIx5gYLyPYJm_8I3yo8FJnMTOokIA";                     // Firebase Project Key
        [HideInInspector] public string dbUrl = "https://victorychallenge-b8854-default-rtdb.firebaseio.com";   // Firebase RealtimeDatabase URL
        private string _idToken;            // Firebase 사용자 토큰
        private string _userId;             // Firebase 유저 아이디
        private string _displayName;        // Firebase 유저 닉네임 
        private string _refreshToken;       // FIrebaes 재발급 토큰
        private DateTime _tokenExpiryTime;  // FIrebase 토큰 인증 만료 시간

        public string IdToken => _idToken;
        public string UserId => _userId;
        public string DisplayName => _displayName;

        [Header("Log-In")]
        [Tooltip("로그인에 필요한 UI")]
        [HideInInspector] public TMP_InputField email;              // 로그인 이메일 입력 필드                     
        [HideInInspector] public TMP_InputField password;           // 로그인 비밀번호 입력 필드                     
        [HideInInspector] public TMP_Text warningLoginText;         // 로그인 실패 메세지 표시                 
        [HideInInspector] public TMP_Text confirmLoginText;         // 로그인 성공 메세지 표시                  

        [Header("Register")]
        [Tooltip("회원가입에 필요한 UI")]
        [HideInInspector] public TMP_InputField usernameRegister;   // 회원가입 닉네임 입력 필드                  
        [HideInInspector] public TMP_InputField emailRegister;      // 회원가입 이메일 입력 필드                  
        [HideInInspector] public TMP_InputField passwordRegister;   // 회원가입 비밀번호 입력 필드                  
        [HideInInspector] public TMP_InputField passwordCheck;      // 비밀번호 체크                 
        [HideInInspector] public TMP_Text warningRegisterText;      // 회원가입 실패 메세지 표시                  
        [HideInInspector] public TMP_Text confirmRegisterText;      // 회원가입 성공 메세지 표시

        #region 로그인
        /// <summary>
        /// 로그인 버튼
        /// </summary>
        /// <param name="onLoginCompleted"> 콜백 메서드 전달 </param>
        public void LoginButton(Action<bool> onLoginCompleted)
        {
            // 로그인 코루틴 시작
            StartCoroutine(Login(email.text, password.text, onLoginCompleted));
        }

        /// <summary>
        /// 로그인 시도
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
                    Debug.Log("로그인 실패");
                }
            });
        }

        /// <summary>
        /// Firebase에 이메일과 비밀번호를 보내서 로그인을 시도
        /// </summary>
        /// <param name="_email"> Firebase에 보낼 이메일 </param>
        /// <param name="_password"> Firebase에 보낼 비밀번호 </param>
        /// <param name="onLoginCompleted"> 로그인 성공여부에 따라 호출되는 콜백 메서드 <param>
        /// <returns></returns>
        IEnumerator Login(string _email, string _password, Action<bool> onLoginCompleted)
        {
            /* email, password, returnSecureToken 값을 포함한 익명 객체를 생성 */
            var payload = new
            {
                email = _email,
                password = _password,
                returnSecureToken = true
            };

            /* Json 문자열로 변환 */
            var jsonPayload = JsonConvert.SerializeObject(payload);

            /* 요청 설정 */
            var request = new RequestHelper
            {
                /* Firebase 인증 엔드포인트 URL */ 
                Uri = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}",
                /* 로그인 요청 */
                Method = "POST",
                /* 본문에 포함될 json */
                BodyString = jsonPayload,
                /* 디버그 모드 활성화 */
                EnableDebug = true
            };

            /* 요청 보내기 */
            RestClient.Request(request, (err, res) =>
            {
                /* 오류 처리 */
                if (err != null)
                {
                    Debug.LogError($"로그인 실패 : {err.Message}");
                    string message = "로그인에 실패했습니다.";
                    
                    /* 응답 오류 처리 */
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
                    /* 서버 오류 처리 */
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

                    /* 오류 메세지 UI에 출력 및 콜백 메서드 호출 */
                    warningLoginText.text = message;
                    onLoginCompleted?.Invoke(false);
                    return;
                }

                /* 로그인 성공 처리, Firebase에서 받은 각 정보들을 저장 */
                var json = JObject.Parse(res.Text);
                _idToken = json["idToken"].ToString();
                _userId = json["localId"].ToString();
                _refreshToken = json["refreshToken"].ToString();
                _tokenExpiryTime = DateTime.Now.AddSeconds(Convert.ToDouble(json["expiresIn"].ToString()));

                Debug.Log($"로그인 성공 : {json["email"]}, {json["password"]}, {json["displayName"]}");

                string shortUID = UIDHelper.GenerateShortUID(_userId);

                /* 사용자 데이터 가져오기 */
                StartCoroutine(FetchUserData(onLoginCompleted, shortUID));
            });
            /* 코루틴 종료 */
            yield return null;
        }

        /// <summary>
        /// DB에 저장된 사용자 데이터 가져오기 
        /// </summary>
        /// <param name="onLoginCompleted"> 로그인 완료 여부 전달할 콜백 메서드 </param>
        /// <param name="shortUID"> 사용자 shortUID </param>
        /// <returns></returns>
        IEnumerator FetchUserData(Action<bool> onLoginCompleted, string shortUID)
        {
            /* 요청 설정 */
            var request = new RequestHelper
            {
                /* 데이터베이스 URL, dbUrl + shortUID, _idToken 필요 */
                Uri = $"{dbUrl}/User/{shortUID}.json?auth={_idToken}",
                /* 데이터를 가져옴 */
                Method = "GET",
                EnableDebug = true
            };

            /* 요청 보내기 */
            RestClient.Request(request, (err, res) =>
            {
                /* 오류 처리 
                 * 요청 중에 오류가 발생했거나
                   응답 텍스트가 비어있거나
                   응답에 "null"인지 확인 */
                if (err != null || string.IsNullOrEmpty(res.Text) || res.Text == "null")
                {
                    onLoginCompleted?.Invoke(false);
                    return;
                }

                /* 응답 처리 */
                var snapshot = JObject.Parse(res.Text);
                if (snapshot != null)
                {
                    /* 사용자 customData, jsonData 추출 */
                    string customData = snapshot["customData"]?.ToString();
                    string userJsonData = snapshot["jsonData"]?.ToString();

                    /* json 데이터를 유저 객체로 변환 */
                    User userData = JsonUtility.FromJson<User>(userJsonData);

                    /* 중복 로그인 확인 */
                    if (userData.isLoggedIn)
                    {
                        warningLoginText.text = " 이미 접속중인 아이디 입니다.";
                        onLoginCompleted?.Invoke(false);
                        return;
                    }

                    /* 사용자 데이터 업데이트 */
                    userData.uid = _userId;
                    userData.shortUID = UIDHelper.GenerateShortUID(_userId);
                    SetDisplayName(userData.userName);
                    userData.isLoggedIn = true;

                    Debug.Log("플레이어 접속 상태 : " + userData.isLoggedIn);

                    DBManager.Instance.WriteUserData(shortUID, userJsonData, customData);

                    /* 사용자 데이터 업데이트 코루틴 호출 */
                    StartCoroutine(UpdateUserData(userData.shortUID, customData, userJsonData, onLoginCompleted));
                }
                else
                {
                    Debug.LogError("사용자 데이터를 찾을 수 없습니다.");
                    onLoginCompleted?.Invoke(false);
                }
            });
            yield return null;
        }

        /// <summary>
        /// 사용자의 데이터를 업데이트
        /// </summary>
        /// <param name="shortUID"> 사용자 shortUID </param>
        /// <param name="customJsonData"> 사용자의 Customizing </param>
        /// <param name="authJsonData"> 사용자의 인증 데이터 </param>
        /// <param name="onLoginCompleted"> 로그인 완료 여부를 콜백 메서드로 전달 </param>
        /// <returns></returns>
        IEnumerator UpdateUserData(string shortUID, string customJsonData, string authJsonData, Action<bool> onLoginCompleted)
        {
            /* 요청 설정 */
            var getRequest = new RequestHelper
            {
                Uri = $"{dbUrl}/User/{shortUID}.json?auth={_idToken}",
                Method = "GET",
                EnableDebug = true,
            };

            /* 요청 보내기 */
            RestClient.Request(getRequest, (getErr, getRes) =>
            {
                /* 오류 처리 */
                if (getErr != null)
                {
                    Debug.LogError($"데이터 업데이트 도중 문제 발생 : {getErr.Message}");
                    onLoginCompleted?.Invoke(false);
                    return;
                }

                /* 기존 데이터 초기화 */
                JObject existingData = new JObject();

                /* 인증 데이터 처리 */
                if (!string.IsNullOrEmpty(authJsonData))                    // 인증 데이터가 비어있는지 확인
                {
                    existingData["jsonData"] = authJsonData;                // 기존 데이터에 인증 데이터 추가
                    JObject isLoggedInData = JObject.Parse(authJsonData);   // 인증 데이터를 Json 객체로 파싱
                    if (isLoggedInData != null)
                    {
                        // 중복 로그인 방지
                        if(isLoggedInData["isLoggedIn"].Value<bool>() == true)
                        {
                            onLoginCompleted?.Invoke(false);
                            return;
                        }

                        isLoggedInData["isLoggedIn"] = true;
                        /* 업데이트 된 인증 데이터를 json 문자열로 변환 후 저장 */
                        existingData["jsonData"] = JsonConvert.SerializeObject(isLoggedInData); 
                    }
                }

                /* Customizing 데이터 처리 */
                if (!string.IsNullOrEmpty(customJsonData))
                {
                    existingData["customData"] = customJsonData;
                }

                /* 업데이트 요청 설정 */
                var putRequest = new RequestHelper
                {
                    Uri = $"{dbUrl}/User/{shortUID}.json?auth={_idToken}",
                    /* 데이터를 업데이트 */
                    Method = "PUT",
                    BodyString = existingData.ToString(),
                    EnableDebug = true
                };

                /* 업데이트 요청 보내기 */
                RestClient.Request(putRequest, (putErr, putRes) =>
                {
                    /* 오류 처리 */
                    if (putErr != null)
                    {
                        Debug.LogError($"데이터 업데이트 중 오류 발생 : {putErr.Message}");
                        onLoginCompleted?.Invoke(false);
                        return;
                    }
                });

                onLoginCompleted?.Invoke(true);
            });

            yield return null;
        }

        /// <summary>
        /// FIrebase 토큰이 만료되었을 때 새로운 토큰 발급
        /// </summary>
        /// <param name="onTokenRefreshed"> 토큰 갱신 여부를 전달할 콜백 메서드 </param>
        /// <returns></returns>
        private IEnumerator RefreshIdToken(Action<bool> onTokenRefreshed)
        {
            /* payload 생성 */
            var refreshPayload = new
            {
                grant_type = "refresh_token",   
                refresh_token = _refreshToken
            };

            /* payload 직렬화 */
            var jsonRefreshPayload = JsonConvert.SerializeObject(refreshPayload);

            /* 요청 설정 */
            var request = new RequestHelper
            {
                Uri = $"https://securetoken.googleapis.com/v1/token?key={apiKey}",
                Method = "POST",
                BodyString = jsonRefreshPayload,
                EnableDebug = true
            };

            /* 요청 전달 */
            RestClient.Request(request, (err, res) =>
            {
                /* 오류 처리 */
                if (err != null)
                {
                    Debug.LogError($"토큰 갱신 실패 {err.Message}");
                    onTokenRefreshed?.Invoke(false);
                    return;
                }

                /* 응답 처리 및 토큰 갱신 */
                var json = JObject.Parse(res.Text);
                _idToken = json["id_token"].ToString();
                _refreshToken = json["refresh_token"].ToString();
                _tokenExpiryTime = DateTime.Now.AddSeconds(Convert.ToDouble(json["expires_in"].ToString()));

                onTokenRefreshed?.Invoke(true);
            });
            yield return null;
        }

        /// <summary>
        /// 토큰이 만료되었는지 확인
        /// </summary>
        /// <param name="onTokenEnsured"> 토큰 유효성 완료 여부 전달할 콜백 메서드 </param>
        public void EnsureIdToken(Action<bool> onTokenEnsured)
        {
            if (IsTokenExpired())   // 토큰이 완료되었는지 확인
            {
                StartCoroutine(RefreshIdToken(onTokenEnsured)); // 토큰 재발급
            }
            else
            {
                onTokenEnsured?.Invoke(true);
            }
        }

        /// <summary>
        /// 토큰 만료 여부 반환
        /// </summary>
        /// <returns></returns>
        private bool IsTokenExpired()
        {
            /* 토큰 만료 시간이 지나면 true 반환 */
            return DateTime.Now >= _tokenExpiryTime;
        }

        /// <summary>
        ///  저장된 토큰 정보 삭제
        /// </summary>
        public void ClearToken()
        {
            _idToken = null;
            _refreshToken = null;
            _userId = null;
            _tokenExpiryTime = DateTime.MinValue;
        }
        #endregion

        #region 로그아웃
        /// <summary>
        /// 로그아웃 및 로그아웃시 정보 저장
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
                    Debug.Log("접속 OFF : " + DBManager.Instance.gameData.users[shortUID].isLoggedIn);
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
        /// <summary>
        /// 회원 가입 버튼
        /// </summary>
        public void RegisterButton()
        {
            StartCoroutine(Register(emailRegister.text, passwordRegister.text, usernameRegister.text));
        }

        /// <summary>
        /// 유저 닉네임 저장
        /// </summary>
        /// <param name="displayName"> 유저 닉네임 </param>
        public void SetDisplayName(string displayName)
        {
            _displayName = displayName;
        }

        /// <summary>
        /// 회원가입 로직
        /// </summary>
        /// <param name="_email"> 회원가입에 필요한 이메일 </param>
        /// <param name="_password"> 회원가입에 필요한 비밀번호 </param>
        /// <param name="_username"> 회원가입에 필요한 유저 닉네임 </param>
        /// <returns></returns>
        IEnumerator Register(string _email, string _password, string _username)
        {
            /* 가입 정보가 비었을 때 */
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
                /* 회원가입 payload */
                var payload = new
                {
                    email = _email,
                    password = _password,
                    returnSecureToken = true
                };

                /* 직렬화 */
                var jsonPayload = JsonConvert.SerializeObject(payload);

                /* 요청 설정 */
                var request = new RequestHelper
                {
                    /* 회원가입을 요청할 http */
                    Uri = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}",
                    Method = "POST",
                    BodyString = jsonPayload,
                    EnableDebug = true,
                    /* 요청 및 응답과 함께 전송되는 메타데이터, 통신 관리 */
                    Headers = new Dictionary<string, string>
                    {
                        {"Content-Type", "application/json" }   // { 데이터 타입, 데이터 파싱 }
                    }
                };

                bool isRequestCompleted = false;    // 요청 여부 false로 설정
                /* 요청 전달 */
                RestClient.Request(request, (err, res) =>
                {
                    /* 오류 처리 */
                    if (err != null)
                    {
                        Debug.LogError($"회원가입 중 오류 발생 : {err.Message}");
                        string message = "회원가입에 실패하였습니다.";

                        /* 응답 오류 처리 */
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
                            /* 서버 오류 처리 */
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
                        isRequestCompleted = true;      // 요청 여부 true
                        return;
                    }

                    /* 회원가입 정보 생성 */
                    var json = JObject.Parse(res.Text);
                    _idToken = json["idToken"].ToString();
                    _refreshToken = json["refreshToken"].ToString();
                    _userId = json["localId"].ToString();
                    SetDisplayName(_username);  // 닉네임 저장

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
                            /* 유저 커스터마이징 정보와 유저 정보 생성 */
                            PlayerCharacterCustomized playerData = new PlayerCharacterCustomized();
                            string customData = playerData.Initialize();
                            DBManager.Instance.customData = customData;

                            string shortUID = UIDHelper.GenerateShortUID(_userId);
                            User newUser = new User(_userId, shortUID, _username, false, 100);
                            string jsonData = JsonUtility.ToJson(newUser);
                            DBManager.Instance.userData = jsonData;

                            DBManager.Instance.WriteUserData(shortUID, jsonData, customData);

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
