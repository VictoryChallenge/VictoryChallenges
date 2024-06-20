using Photon.Pun;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using VictoryChallenge.KJ.Photon;
using VictoryChallenge.KJ.Database;
using VictoryChallenge.Customize;
using Firebase.Extensions;
using Newtonsoft.Json;

namespace VictoryChallenge.KJ.Auth
{
    /// <summary>
    /// 로그인
    /// </summary>
    public class Authentication : SingletonLazy<Authentication>
    {
        [Header("Firebase")]
        [Tooltip("파이어베이스 인증에 필요한 변수들")]
        public DependencyStatus dependencyStatus;                   // Firebase 초기화 후 상태 확인
        public FirebaseAuth _auth { get; private set; }             // 인증(로그인)
        public FirebaseUser _user { get; private set; }             // 인증이 완료된 유저 정보

        [Header("Log-In")]
        [Tooltip("로그인에 필요한 UI")]
        [HideInInspector] public TMP_InputField email;                                // E-mail
        [HideInInspector] public TMP_InputField password;                             // Password
        [HideInInspector] public TMP_Text warningLoginText;                           // 오류 메세지
        [HideInInspector] public TMP_Text confirmLoginText;                           // 성공시 나타나는 메세지

        [Header("Register")]
        [Tooltip("회원가입에 필요한 UI")]
        [HideInInspector] public TMP_InputField usernameRegister;                     // username 입력
        [HideInInspector] public TMP_InputField emailRegister;                        // email 입력
        [HideInInspector] public TMP_InputField passwordRegister;                     // password 입력
        [HideInInspector] public TMP_InputField passwordCheck;                        // password 입력 체크
        [HideInInspector] public TMP_Text warningRegisterText;                        // 오류 메세지
        [HideInInspector] public TMP_Text confirmRegisterText;                        // 성공시 나타나는 메세지

        private DatabaseReference _databaseReference;               // 데이터베이스의 특정 위치 참조

        /// <summary>
        /// 의존성 상태 확인 후 초기화
        /// </summary>
        void Awake()
        {
            // Firebase 의존성 상태 체크
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = task.Result;

                if (dependencyStatus == DependencyStatus.Available)
                {
                    FirebaseApp app = FirebaseApp.DefaultInstance;
                    if (app.Options.DatabaseUrl == null)
                    {
                        app.Options.DatabaseUrl = new Uri("https://victorychallenge-b8854-default-rtdb.firebaseio.com/");
                    }
                    _auth = FirebaseAuth.GetAuth(app);
                    _databaseReference = FirebaseDatabase.GetInstance(app).RootReference;

                    Debug.Log("Firebase 초기화 성공");
                }
                else
                {
                    Debug.LogError("Firebase 초기화 실패" + dependencyStatus);
                }
            });
        }

        #region 로그인 및 회원가입
        /// <summary>
        /// Firebase 초기화
        /// </summary>
        public void InitializeFirebase()
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            if (app.Options.DatabaseUrl == null)
            {
                app.Options.DatabaseUrl = new Uri("https://victorychallenge-b8854-default-rtdb.firebaseio.com/");
            }
            _auth = FirebaseAuth.GetAuth(app);
            _databaseReference = FirebaseDatabase.GetInstance(app).RootReference;
        }

        /// <summary>
        /// 로그인 버튼
        /// </summary>
        /// <param name="onLoginCompleted">로그인 작업이 완료되었을 때 호출되는 콜백</param>
        public void LoginButton(Action<bool> onLoginCompleted)
        {
            StartCoroutine(Login(email.text, password.text, onLoginCompleted));
        }

        /// <summary>
        /// 로그인 결과에 따라 UI 이동
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
        /// 로그인 로직
        /// </summary>
        /// <param name="_email">이메일</param>
        /// <param name="_password">비밀번호</param>
        /// <param name="onLoginCompleted">로그인 작업이 완료되었을 때 호출되는 콜백</param>
        /// <returns></returns>
        IEnumerator Login(string _email, string _password, Action<bool> onLoginCompleted)
        {
            var LoginTask = _auth.SignInWithEmailAndPasswordAsync(_email, _password);
            // LoginTask.IsCompleted가 참이 될 때 까지 기다림
            yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

            if (LoginTask.Exception != null)
            {
                // 예외 처리
                Debug.LogError(message: $"예외 발생 {LoginTask.Exception}");
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;

                string message = "로그인에 실패했습니다.";

                if (firebaseEx != null)
                {
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;


                    switch (errorCode)
                    {
                        case AuthError.MissingEmail:
                            message = "이메일을 입력해주세요";
                            break;

                        case AuthError.MissingPassword:
                            message = "비밀번호를 입력해주세요.";
                            break;

                        case AuthError.InvalidEmail:
                            message = "잘못된 이메일입니다. 다시 확인해주세요.";
                            break;

                        case AuthError.WrongPassword:
                            message = "잘못된 비밀번호입니다. 다시 확인해주세요.";
                            break;

                        case AuthError.UserNotFound:
                            message = "계정이 이미 존재합니다.";
                            break;

                        case AuthError.NetworkRequestFailed:
                            message = "네트워크 오류입니다. 네트워크 연결을 확인하세요.";
                            break;
                    }
                }
                else
                {
                    message = $"알 수 없는 오류가 발생하였습니다. : {LoginTask.Exception.GetBaseException().Message}";
                }
                warningLoginText.text = message;
                onLoginCompleted?.Invoke(false);
            }
            else
            {
                _user = LoginTask.Result.User;
                Debug.LogFormat($"로그인 성공 : {_user.Email}, {_user.DisplayName}");

                string shortUID = UIDHelper.GenerateShortUID(_user.UserId);
                DatabaseReference userRef = FirebaseDatabase.DefaultInstance.GetReference("User").Child(shortUID);

                var userTask = userRef.GetValueAsync();
                yield return new WaitUntil(predicate: () => userTask.IsCompleted);

                if (userTask.Exception != null)
                {
                    Debug.Log("데이터 불러오는 중 에러 발생" + userTask.Exception);
                    onLoginCompleted?.Invoke(false);
                }
                else
                {
                    DataSnapshot snapshot = userTask.Result;
                    if (snapshot.Exists)
                    {
                        string json = snapshot.GetRawJsonValue();
                        string customData = snapshot.Child("customData").Value.ToString();
                        string userjsonData = snapshot.Child("jsonData").Value.ToString();

                        User userData = JsonUtility.FromJson<User>(userjsonData);

                        if (userData.isLoggedIn)
                        {
                            warningLoginText.text = "이미 접속중인 아이디 입니다.";
                            yield break;
                        }

                        userData.uid = _user.UserId;
                        userData.shortUID = shortUID;
                        userData.userName = _user.DisplayName;
                        userData.isLoggedIn = true;

                        Debug.Log("접속중 ON : " + userData.isLoggedIn);

                        string updateJsonData = JsonUtility.ToJson(userData);
                        DatabaseManager.Instance.WriteUserData(userData.shortUID, updateJsonData, customData);

                        warningLoginText.text = "";
                        confirmLoginText.text = "로그인에 성공했습니다.";

                        onLoginCompleted?.Invoke(true);
                    }
                    else
                    {
                        Debug.Log("스냅샷" + snapshot);
                        Debug.LogError("사용자 데이터를 찾을 수 없습니다.");
                        onLoginCompleted?.Invoke(false);
                    }
                }
            }
        }

        /// <summary>
        /// 로그 아웃 로직
        /// </summary>
        public void LogOut()
        {
            if (_user != null )
            {
                //// 로그아웃 정보 업데이트 (데이터베이스)
                string shortUID = UIDHelper.GenerateShortUID(_user.UserId);
                DatabaseManager.Instance.gameData.users[shortUID].isLoggedIn = false;
                User userData = DatabaseManager.Instance.gameData.users[shortUID];
                string json = JsonUtility.ToJson(userData);

                DatabaseManager.Instance.SignOutProcess(shortUID, json, DatabaseManager.Instance.customData);
                Debug.Log("접속중 OFF : " + DatabaseManager.Instance.gameData.users[shortUID].isLoggedIn);
                    
                // 로그아웃과 게임종료가 데이터 업데이트 이후에 이루어져야함
            }
            else
            {
                Debug.Log("로그인된 계정이 없습니다.");
            }
        }

        /// <summary>
        /// 회원가입 버튼
        /// </summary>
        public void RegisterButton()
        {
            StartCoroutine(Register(emailRegister.text, passwordRegister.text, usernameRegister.text));
        }

        /// <summary>
        /// 회원가입 로직
        /// </summary>
        /// <param name="_email">이메일</param>
        /// <param name="_password">비밀번호</param>
        /// <param name="_username">닉네임</param>
        /// <returns></returns>
        IEnumerator Register(string _email, string _password, string _username)
        {
            if (string.IsNullOrWhiteSpace(_username))
            {
                warningRegisterText.text = "닉네임을 입력하세요.";
            }
            else if (passwordRegister.text != passwordCheck.text)
            {
                warningRegisterText.text = "비밀번호를 확인하세요.";
            }
            else
            {
                var RegisterTask = _auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
                // LoginTask.IsCompleted가 참이 될 때 까지 기다림
                yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

                if (RegisterTask.Exception != null)
                {
                    // 예외 처리
                    Debug.LogError(message: $"예외 발생 {RegisterTask.Exception}");
                    FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                    string message = "회원가입에 실패했습니다.";
                    switch (errorCode)
                    {
                        case AuthError.MissingEmail:
                            message = "이메일을 입력해주세요.";
                            break;

                        case AuthError.MissingPassword:
                            message = "비밀번호를 입력해주세요.";
                            break;

                        case AuthError.InvalidEmail:
                            message = "잘못된 이메일입니다. 다시 확인해주세요.";
                            break;

                        case AuthError.WrongPassword:
                            message = "잘못된 비밀번호입니다. 다시 확인해주세요.";
                            break;

                        case AuthError.UserNotFound:
                            message = "이미 존재하는 계정입니다.";
                            break;

                        case AuthError.EmailAlreadyInUse:
                            message = "이메일이 이미 사용중입니다.";
                            break;

                        case AuthError.WeakPassword:
                            message = "비밀번호 보안이 약합니다.";
                            break;

                        case AuthError.NetworkRequestFailed:
                            message = "네트워크 오류입니다. 네트워크 연결을 확인하세요.";
                            break;
                    }
                    warningRegisterText.text = message;
                }
                else
                {
                    // 회원가입 성공
                    _user = RegisterTask.Result.User;

                    if (_user != null)
                    {
                        UserProfile profile = new UserProfile { DisplayName = _username };

                        var ProfileTask = _user.UpdateUserProfileAsync(profile);

                        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                        if (ProfileTask.Exception != null)
                        {
                            // 사용자 정보를 불러올 때 예외가 발생하면 처리
                            Debug.LogError(message: $"사용자 프로필 정보를 업데이트 하는데 예외가 발생했습니다. {ProfileTask.Exception}");
                            FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                            warningRegisterText.text = "닉네임을 확인하세요.";
                        }
                        else
                        {
                            // 회원가입 성공
                            Debug.LogFormat("회원가입이 성공적으로 이루어졌습니다." + _user.DisplayName);
                            confirmRegisterText.text = "회원가입이 성공적으로 이루어졌습니다.";
                            warningRegisterText.text = "";

                            PlayerCharacterCustomized playerData = new PlayerCharacterCustomized();
                            string customData = playerData.Initialize();
                            DatabaseManager.Instance.customData = customData;

                            string shortUID = UIDHelper.GenerateShortUID(_user.UserId);
                            User newUser = new User(_user.UserId, shortUID, _username, false, 100, 0);
                            string jsonData = JsonUtility.ToJson(newUser);
                            DatabaseManager.Instance.userData = jsonData;
                            DatabaseManager.Instance.WriteUserData(newUser.shortUID, jsonData, customData);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
