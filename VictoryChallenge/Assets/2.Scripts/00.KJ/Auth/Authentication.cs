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
    /// �α���
    /// </summary>
    public class Authentication : SingletonLazy<Authentication>
    {
        [Header("Firebase")]
        [Tooltip("���̾�̽� ������ �ʿ��� ������")]
        public DependencyStatus dependencyStatus;                   // Firebase �ʱ�ȭ �� ���� Ȯ��
        public FirebaseAuth _auth { get; private set; }             // ����(�α���)
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

        private DatabaseReference _databaseReference;               // �����ͺ��̽��� Ư�� ��ġ ����

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
                    FirebaseApp app = FirebaseApp.DefaultInstance;
                    if (app.Options.DatabaseUrl == null)
                    {
                        app.Options.DatabaseUrl = new Uri("https://victorychallenge-b8854-default-rtdb.firebaseio.com/");
                    }
                    _auth = FirebaseAuth.GetAuth(app);
                    _databaseReference = FirebaseDatabase.GetInstance(app).RootReference;

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
            FirebaseApp app = FirebaseApp.DefaultInstance;
            if (app.Options.DatabaseUrl == null)
            {
                app.Options.DatabaseUrl = new Uri("https://victorychallenge-b8854-default-rtdb.firebaseio.com/");
            }
            _auth = FirebaseAuth.GetAuth(app);
            _databaseReference = FirebaseDatabase.GetInstance(app).RootReference;
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
                    message = $"�� �� ���� ������ �߻��Ͽ����ϴ�. : {LoginTask.Exception.GetBaseException().Message}";
                }
                warningLoginText.text = message;
                onLoginCompleted?.Invoke(false);
            }
            else
            {
                _user = LoginTask.Result.User;
                Debug.LogFormat($"�α��� ���� : {_user.Email}, {_user.DisplayName}");

                string shortUID = UIDHelper.GenerateShortUID(_user.UserId);
                DatabaseReference userRef = FirebaseDatabase.DefaultInstance.GetReference("User").Child(shortUID);

                var userTask = userRef.GetValueAsync();
                yield return new WaitUntil(predicate: () => userTask.IsCompleted);

                if (userTask.Exception != null)
                {
                    Debug.Log("������ �ҷ����� �� ���� �߻�" + userTask.Exception);
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
                            warningLoginText.text = "�̹� �������� ���̵� �Դϴ�.";
                            yield break;
                        }

                        userData.uid = _user.UserId;
                        userData.shortUID = shortUID;
                        userData.userName = _user.DisplayName;
                        userData.isLoggedIn = true;

                        Debug.Log("������ ON : " + userData.isLoggedIn);

                        string updateJsonData = JsonUtility.ToJson(userData);
                        DatabaseManager.Instance.WriteUserData(userData.shortUID, updateJsonData, customData);

                        warningLoginText.text = "";
                        confirmLoginText.text = "�α��ο� �����߽��ϴ�.";

                        onLoginCompleted?.Invoke(true);
                    }
                    else
                    {
                        Debug.Log("������" + snapshot);
                        Debug.LogError("����� �����͸� ã�� �� �����ϴ�.");
                        onLoginCompleted?.Invoke(false);
                    }
                }
            }
        }

        /// <summary>
        /// �α� �ƿ� ����
        /// </summary>
        public void LogOut()
        {
            if (_user != null )
            {
                //// �α׾ƿ� ���� ������Ʈ (�����ͺ��̽�)
                string shortUID = UIDHelper.GenerateShortUID(_user.UserId);
                DatabaseManager.Instance.gameData.users[shortUID].isLoggedIn = false;
                User userData = DatabaseManager.Instance.gameData.users[shortUID];
                string json = JsonUtility.ToJson(userData);

                DatabaseManager.Instance.SignOutProcess(shortUID, json, DatabaseManager.Instance.customData);
                Debug.Log("������ OFF : " + DatabaseManager.Instance.gameData.users[shortUID].isLoggedIn);
                    
                // �α׾ƿ��� �������ᰡ ������ ������Ʈ ���Ŀ� �̷��������
            }
            else
            {
                Debug.Log("�α��ε� ������ �����ϴ�.");
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
                            confirmRegisterText.text = "ȸ�������� ���������� �̷�������ϴ�.";
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
