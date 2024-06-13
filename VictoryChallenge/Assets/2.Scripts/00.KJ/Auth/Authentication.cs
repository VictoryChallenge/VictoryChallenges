using Firebase;
using Firebase.Auth;
//using Firebase.Database;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;

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
        private FirebaseAuth _auth;                                 // 인증(로그인)
        public FirebaseUser _user { get; private set; }             // 인증이 완료된 유저 정보

        [Header("Log-In")]
        [Tooltip("로그인에 필요한 UI")]
        public TMP_InputField email;                                // E-mail
        public TMP_InputField password;                             // Password
        public TMP_Text warningLoginText;                           // 오류 메세지
        public TMP_Text confirmLoginText;                           // 성공시 나타나는 메세지

        [Header("Register")]
        [Tooltip("회원가입에 필요한 UI")]
        public TMP_InputField usernameRegister;                     // username 입력
        public TMP_InputField emailRegister;                        // email 입력
        public TMP_InputField passwordRegister;                     // password 입력
        public TMP_InputField passwordCheck;                        // password 입력 체크
        public TMP_Text warningRegisterText;                        // 오류 메세지
        public TMP_Text confirmRegisterText;                        // 성공시 나타나는 메세지

        //private DatabaseReference _databaseReference;               // 데이터베이스의 특정 위치 참조
        [Header("테스트")]
        private bool _isActiveL = false;
        private bool _isActiveR = false;
        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private GameObject _registerPanel;

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
                    InitializeFirebase();
                    //_databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                    Debug.Log("Firebase 초기화 성공");
                }
                else
                {
                    Debug.LogError("Firebase 초기화 실패" + dependencyStatus);
                }
            });
        }
        #region SetActive
        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                ExitUI();
            }
        }

        public void OpenLoginUI()
        {
            _isActiveL = true;
            Debug.Log("로그인창 활성화 ");
            _loginPanel.SetActive(true);
        }

        public void OpenRegisterUI()
        {
            _isActiveR = true;
            Debug.Log("회원가입창 활성화");
            _registerPanel.SetActive(true);
        }

        public void ExitUI()
        {
            if (_isActiveL == true)
            {
                _isActiveL = false;
                _loginPanel.SetActive(false);
            }
            else if (_isActiveR == true)
            {
                _isActiveR = false;
                _registerPanel.SetActive(false);
            }
        }
        #endregion

        /// <summary>
        /// Firebase 초기화
        /// </summary>
        public void InitializeFirebase()
        {
            _auth = FirebaseAuth.DefaultInstance;
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
                    Debug.Log($"로그인 정보  {email.text}, {usernameRegister.text}, {password.text}");
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
            _user = LoginTask.Result.User;

            if (LoginTask.Exception != null)
            {
                // 예외 처리
                Debug.LogWarning(message: $"예외 발생 {LoginTask.Exception}");
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "로그인 실패";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Enter your e-mail";
                        break;

                    case AuthError.MissingPassword:
                        message = "Enter your password";
                        break;

                    case AuthError.InvalidEmail:
                        message = "Invalid e-mail, Check again";
                        break;

                    case AuthError.WrongPassword:
                        message = "Wrong password, Check again";
                        break;

                    case AuthError.UserNotFound:
                        message = "Account not exist";
                        break;

                    case AuthError.NetworkRequestFailed:
                        message = "NetworkError";
                        break;
                }
                warningLoginText.text = message;
                onLoginCompleted?.Invoke(false);
            }
            else
            {
                // 로그인 성공했을 경우
                Debug.LogFormat("로그인 성공 : {0}, {1} ", _user.Email, _user.DisplayName);
                warningLoginText.text = "";
                confirmLoginText.text = "Login successful";
                // yield return DB로 보내야함

                onLoginCompleted?.Invoke(true);
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
                warningRegisterText.text = "Enter your username";
            }
            else if (passwordRegister.text != passwordCheck.text)
            {
                warningRegisterText.text = "Check your password";
            }
            else
            {
                var RegisterTask = _auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
                // LoginTask.IsCompleted가 참이 될 때 까지 기다림
                yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

                if (RegisterTask.Exception != null)
                {
                    // 예외 처리
                    Debug.LogWarning(message: $"예외 발생 {RegisterTask.Exception}");
                    FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                    string message = "register Fail";
                    switch (errorCode)
                    {
                        case AuthError.MissingEmail:
                            message = "Enter your e-mail";
                            break;

                        case AuthError.MissingPassword:
                            message = "Enter your password";
                            break;

                        case AuthError.InvalidEmail:
                            message = "Invalid e-mail, Check again";
                            break;

                        case AuthError.WrongPassword:
                            message = "wrong password, Check again";
                            break;

                        case AuthError.UserNotFound:
                            message = "Account not exist";
                            break;

                        case AuthError.EmailAlreadyInUse:
                            message = "E-mail already in used";
                            break;

                        case AuthError.WeakPassword:
                            message = "Weak password";
                            break;

                        case AuthError.NetworkRequestFailed:
                            message = "Network error";
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
                            Debug.LogWarning(message: $"사용자 프로필 정보를 업데이트 하는데 예외가 발생했습니다. {ProfileTask.Exception}");
                            FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                            warningRegisterText.text = "Failed username";
                        }
                        else
                        {
                            // 회원가입 성공
                            Debug.Log("회원가입이 성공적으로 이루어졌습니다." + _user.DisplayName);
                            confirmRegisterText.text = "Register successful";
                            warningRegisterText.text = "";
                        }
                    }
                }
            }
        }
    }

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
}
