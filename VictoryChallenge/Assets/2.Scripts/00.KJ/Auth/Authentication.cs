using Firebase;
using Firebase.Auth;
using Photon.Pun;

//using Firebase.Database;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using VictoryChallenge.KJ.Photon;

namespace VictoryChallenge.KJ.Auth
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
        [HideInInspector] public TMP_InputField email;                                // E-mail
        [HideInInspector] public TMP_InputField password;                             // Password
        [HideInInspector] public TMP_Text warningLoginText;                           // ���� �޼���
        [HideInInspector] public TMP_Text confirmLoginText;                           // ������ ��Ÿ���� �޼���

        [Header("Register")]
        [Tooltip("ȸ�����Կ� �ʿ��� UI")]
        [HideInInspector] public TMP_InputField usernameRegister;                     // username �Է�
        [HideInInspector] public TMP_InputField emailRegister;                        // email �Է�
        [HideInInspector] public TMP_InputField passwordRegister;                     // password �Է�
        [HideInInspector] public TMP_InputField passwordCheck;                        // password �Է� üũ
        [HideInInspector] public TMP_Text warningRegisterText;                        // ���� �޼���
        [HideInInspector] public TMP_Text confirmRegisterText;                        // ������ ��Ÿ���� �޼���

        //private DatabaseReference _databaseReference;               // �����ͺ��̽��� Ư�� ��ġ ����

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

        #region �α��� �� ȸ������
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

            if (LoginTask.Exception != null)
            {
                // ���� ó��
                Debug.LogError(message: $"���� �߻� {LoginTask.Exception}");
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;

                string message = "�α��ο� �����߽��ϴ�.";

                if (firebaseEx != null)
                {
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;


                    switch (errorCode)
                    {
                        case AuthError.MissingEmail:
                            message = "�̸����� �Է����ּ���";
                            break;

                        case AuthError.MissingPassword:
                            message = "��й�ȣ�� �Է����ּ���.";
                            break;

                        case AuthError.InvalidEmail:
                            message = "�߸��� �̸����Դϴ�. �ٽ� Ȯ�����ּ���.";
                            break;

                        case AuthError.WrongPassword:
                            message = "�߸��� ��й�ȣ�Դϴ�. �ٽ� Ȯ�����ּ���.";
                            break;

                        case AuthError.UserNotFound:
                            message = "������ �̹� �����մϴ�.";
                            break;

                        case AuthError.NetworkRequestFailed:
                            message = "��Ʈ��ũ �����Դϴ�. ��Ʈ��ũ ������ Ȯ���ϼ���.";
                            break;
                    }
                }
                else
                {
                    message = $"�˼� ���� ������ �߻��Ͽ����ϴ�. : {LoginTask.Exception.GetBaseException().Message}";
                }
                warningLoginText.text = message;
                onLoginCompleted?.Invoke(false);
            }
            else
            {
                try
                {
                    _user = LoginTask.Result.User;
                    // �α��� �������� ���
                    Debug.LogFormat($"�α��� ���� : {_user.Email}, {_user.DisplayName}");
                    warningLoginText.text = "";
                    confirmLoginText.text = "�α��ο� �����߽��ϴ�.";
                    // yield return DB�� ��������

                    onLoginCompleted?.Invoke(true);
                }
                catch (Exception ex)
                {
                    Debug.Log("���� �߻�");
                    warningLoginText.text = "����� ���� �������� ���� ������ �߻��߽��ϴ�.";
                    onLoginCompleted?.Invoke(false);
                }
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
                warningRegisterText.text = "�г����� �Է��ϼ���.";
            }
            else if (passwordRegister.text != passwordCheck.text)
            {
                warningRegisterText.text = "��й�ȣ�� Ȯ���ϼ���.";
            }
            else
            {
                var RegisterTask = _auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
                // LoginTask.IsCompleted�� ���� �� �� ���� ��ٸ�
                yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

                if (RegisterTask.Exception != null)
                {
                    // ���� ó��
                    Debug.LogError(message: $"���� �߻� {RegisterTask.Exception}");
                    FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                    string message = "ȸ�����Կ� �����߽��ϴ�.";
                    switch (errorCode)
                    {
                        case AuthError.MissingEmail:
                            message = "�̸����� �Է����ּ���.";
                            break;

                        case AuthError.MissingPassword:
                            message = "��й�ȣ�� �Է����ּ���.";
                            break;

                        case AuthError.InvalidEmail:
                            message = "�߸��� �̸����Դϴ�. �ٽ� Ȯ�����ּ���.";
                            break;

                        case AuthError.WrongPassword:
                            message = "�߸��� ��й�ȣ�Դϴ�. �ٽ� Ȯ�����ּ���.";
                            break;

                        case AuthError.UserNotFound:
                            message = "�̹� �����ϴ� �����Դϴ�.";
                            break;

                        case AuthError.EmailAlreadyInUse:
                            message = "�̸����� �̹� ������Դϴ�.";
                            break;

                        case AuthError.WeakPassword:
                            message = "��й�ȣ ������ ���մϴ�.";
                            break;

                        case AuthError.NetworkRequestFailed:
                            message = "��Ʈ��ũ �����Դϴ�. ��Ʈ��ũ ������ Ȯ���ϼ���.";
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
                            Debug.LogError(message: $"����� ������ ������ ������Ʈ �ϴµ� ���ܰ� �߻��߽��ϴ�. {ProfileTask.Exception}");
                            FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                            warningRegisterText.text = "�г����� Ȯ���ϼ���.";
                        }
                        else
                        {
                            // ȸ������ ����
                            Debug.LogFormat("ȸ�������� ���������� �̷�������ϴ�." + _user.DisplayName);
                            PhotonManager.Instance.SetPlayerNickname(_username);
                            confirmRegisterText.text = "ȸ�������� ���������� �̷�������ϴ�.";
                            warningRegisterText.text = "";
                        }
                    }
                }
            }
        }
        #endregion

        #region ShortUID
        #endregion
    }
}
