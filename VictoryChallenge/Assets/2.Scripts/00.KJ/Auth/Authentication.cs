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
using static VictoryChallenge.Customize.PlayerCharacterCustomized;
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

                        User userData = JsonUtility.FromJson<User>(json);
                        userData.uid = _user.UserId;
                        userData.shortUID = shortUID;
                        userData.userName = _user.DisplayName;

                        string updateJsonData = JsonUtility.ToJson(userData);
                        DatabaseManager.Instance.WriteUserData(userData.shortUID, true, updateJsonData, customData);

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
                //try
                //{
                //    _user = LoginTask.Result.User;
                //    // �α��� �������� ���
                //    Debug.LogFormat($"�α��� ���� : {_user.Email}, {_user.DisplayName}");
                //    warningLoginText.text = "";
                //    confirmLoginText.text = "�α��ο� �����߽��ϴ�.";

                //    // �α��� ���� ������Ʈ (�����ͺ��̽�)
                //    DatabaseManager.Instance.WriteUserData(_user.UserId, true, "jsonData");

                //    onLoginCompleted?.Invoke(true);
                //}
                //catch (Exception ex)
                //{
                //    Debug.Log("���� �߻�");
                //    warningLoginText.text = "����� ���� �������� ���� ������ �߻��߽��ϴ�.";
                //    onLoginCompleted?.Invoke(false);
                //}
            }
        }

        /// <summary>
        /// �α� �ƿ� ����
        /// </summary>
        public void LogOut()
        {
            if (_user != null )
            {
                // �α׾ƿ� ���� ������Ʈ (�����ͺ��̽�)
                string shortUID = UIDHelper.GenerateShortUID(_user.UserId);
                DatabaseManager.Instance.WriteUserData(shortUID, false, "", "");

                _auth.SignOut();
                Debug.Log("�α׾ƿ� ����");
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
                            PhotonManager.Instance.SetPlayerNickname(_username);
                            confirmRegisterText.text = "ȸ�������� ���������� �̷�������ϴ�.";
                            warningRegisterText.text = "";

                            PlayerCharacterCustomized playerData = new PlayerCharacterCustomized();
                            string customData = playerData.Initialize();

                            string shortUID = UIDHelper.GenerateShortUID(_user.UserId);
                            User newUser = new User(_user.UserId, shortUID, _username, 100, 0);
                            string jsonData = JsonUtility.ToJson(newUser);
                            DatabaseManager.Instance.WriteUserData(newUser.shortUID, false, jsonData, customData);
                        }
                    }
                }
            }
        }
        #endregion

        private IEnumerator C_LoadjsonData(string shortUID)
        {
            string userData = "";
            DatabaseReference db = FirebaseDatabase.DefaultInstance.GetReference("User");
            db.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("ReadData is Faulted");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log("ChilderenCount" + snapshot.ChildrenCount);

                    foreach (var child in snapshot.Children)
                    {
                        if (child.Key == shortUID)
                        {
                            Debug.Log("child.Value.ToString() : " + child.ToString());
                            userData = child.Child("customData").Value.ToString();
                        }
                    }
                }
            });
            yield return new WaitUntil(() => !string.IsNullOrEmpty(userData));
            
        }

    }
}
