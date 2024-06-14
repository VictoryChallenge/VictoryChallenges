using Firebase;
using Firebase.Auth;
//using Firebase.Database;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using VictoryChallenge.KJ.Photon;

namespace VictoryChallenge.KJ.Auth
{
    /// <summary>
    /// �α���
    /// </summary>
    public class Authentication : SingletonLazy<Authentication>
    {
        [Header("Firebase")]
        [Tooltip("���̾�̽� ������ �ʿ��� ������")]
        public DependencyStatus dependencyStatus;                   // Firebase �ʱ�ȭ �� ���� Ȯ��
        private FirebaseAuth _auth;                                 // ����(�α���)
        public FirebaseUser _user { get; private set; }             // ������ �Ϸ�� ���� ����

        [Header("Log-In")]
        [Tooltip("�α��ο� �ʿ��� UI")]
        public TMP_InputField email;                                // E-mail
        public TMP_InputField password;                             // Password
        public TMP_Text warningLoginText;                           // ���� �޼���
        public TMP_Text confirmLoginText;                           // ������ ��Ÿ���� �޼���

        [Header("Register")]
        [Tooltip("ȸ�����Կ� �ʿ��� UI")]
        public TMP_InputField usernameRegister;                     // username �Է�
        public TMP_InputField emailRegister;                        // email �Է�
        public TMP_InputField passwordRegister;                     // password �Է�
        public TMP_InputField passwordCheck;                        // password �Է� üũ
        public TMP_Text warningRegisterText;                        // ���� �޼���
        public TMP_Text confirmRegisterText;                        // ������ ��Ÿ���� �޼���

        //private DatabaseReference _databaseReference;               // �����ͺ��̽��� Ư�� ��ġ ����
        [Header("�׽�Ʈ")]
        private bool _isActiveL = false;
        private bool _isActiveR = false;
        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private GameObject _registerPanel;

        /// <summary>
        /// ������ ���� Ȯ�� �� �ʱ�ȭ
        /// </summary>
        void Awake()
        {
            // Firebase ������ ���� üũ
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = task.Result;

                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                    //_databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                    Debug.Log("Firebase �ʱ�ȭ ����");
                }
                else
                {
                    Debug.LogError("Firebase �ʱ�ȭ ����" + dependencyStatus);
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
            Debug.Log("�α���â Ȱ��ȭ ");
            _loginPanel.SetActive(true);
        }

        public void OpenRegisterUI()
        {
            _isActiveR = true;
            Debug.Log("ȸ������â Ȱ��ȭ");
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
        /// Firebase �ʱ�ȭ
        /// </summary>
        public void InitializeFirebase()
        {
            _auth = FirebaseAuth.DefaultInstance;
        }

        /// <summary>
        /// �α��� ��ư
        /// </summary>
        /// <param name="onLoginCompleted">�α��� �۾��� �Ϸ�Ǿ��� �� ȣ��Ǵ� �ݹ�</param>
        public void LoginButton(Action<bool> onLoginCompleted)
        {
            StartCoroutine(Login(email.text, password.text, onLoginCompleted));
        }

        /// <summary>
        /// �α��� ����� ���� UI �̵�
        /// </summary>
        public void AttemptLogin()
        {
            LoginButton(Result =>
            {
                if (Result)
                {
                    Debug.Log($"�α��� ����  {email.text}, {usernameRegister.text}, {password.text}");
                }
                else
                {
                    Debug.Log("�α��� ����");
                }
            });
        }

        /// <summary>
        /// �α��� ����
        /// </summary>
        /// <param name="_email">�̸���</param>
        /// <param name="_password">��й�ȣ</param>
        /// <param name="onLoginCompleted">�α��� �۾��� �Ϸ�Ǿ��� �� ȣ��Ǵ� �ݹ�</param>
        /// <returns></returns>
        IEnumerator Login(string _email, string _password, Action<bool> onLoginCompleted)
        {
            var LoginTask = _auth.SignInWithEmailAndPasswordAsync(_email, _password);
            // LoginTask.IsCompleted�� ���� �� �� ���� ��ٸ�
            yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
            _user = LoginTask.Result.User;

            if (LoginTask.Exception != null)
            {
                // ���� ó��
                Debug.LogWarning(message: $"���� �߻� {LoginTask.Exception}");
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "�α��� ����";
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
                // �α��� �������� ���
                Debug.LogFormat("�α��� ���� : {0}, {1} ", _user.Email, _user.DisplayName);
                warningLoginText.text = "";
                confirmLoginText.text = "Login successful";
                // yield return DB�� ��������

                onLoginCompleted?.Invoke(true);
            }
        }

        /// <summary>
        /// ȸ������ ��ư
        /// </summary>
        public void RegisterButton()
        {
            StartCoroutine(Register(emailRegister.text, passwordRegister.text, usernameRegister.text));
        }

        /// <summary>
        /// ȸ������ ����
        /// </summary>
        /// <param name="_email">�̸���</param>
        /// <param name="_password">��й�ȣ</param>
        /// <param name="_username">�г���</param>
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
                // LoginTask.IsCompleted�� ���� �� �� ���� ��ٸ�
                yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

                if (RegisterTask.Exception != null)
                {
                    // ���� ó��
                    Debug.LogWarning(message: $"���� �߻� {RegisterTask.Exception}");
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
                    // ȸ������ ����
                    _user = RegisterTask.Result.User;

                    if (_user != null)
                    {
                        UserProfile profile = new UserProfile { DisplayName = _username };

                        var ProfileTask = _user.UpdateUserProfileAsync(profile);

                        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                        if (ProfileTask.Exception != null)
                        {
                            // ����� ������ �ҷ��� �� ���ܰ� �߻��ϸ� ó��
                            Debug.LogWarning(message: $"����� ������ ������ ������Ʈ �ϴµ� ���ܰ� �߻��߽��ϴ�. {ProfileTask.Exception}");
                            FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                            warningRegisterText.text = "Failed username";
                        }
                        else
                        {
                            // ȸ������ ����
                            Debug.Log("ȸ�������� ���������� �̷�������ϴ�." + _user.DisplayName);
                            PhotonManager.Instance.SetPlayerNickname(_username);
                            confirmRegisterText.text = "Register successful";
                            warningRegisterText.text = "";
                        }
                    }
                }
            }
        }
    }

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
}
