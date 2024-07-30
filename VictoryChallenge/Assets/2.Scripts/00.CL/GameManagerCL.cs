using Photon.Pun;
using ExitGames.Client.Photon;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;
using VictoryChallenge.KJ.Room;
using System.Collections.Generic;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Unity.VisualScripting;


namespace VictoryChallenge.Scripts.CL
{
    // �� ��ũ��Ʈ�� ��� �÷��̾ ���� �ε��ߴ��� Ȯ���ϰ�, �� �� ������ �����ϴ� ������ �մϴ�.
    public class GameManagerCL : MonoBehaviourPunCallbacks
    {
        // random scene list
        private bool _isFirstGoalIned;
        private bool _isFinished = false;
        private int _nextSceneNum;
        private List<int> _round2List;
        private List<int> _round3List;

        // Ŀ���� ������Ƽ �ѹ��� �ҷ����� ����
        private bool isGameStarted;
        
        // PlayerActive
        private bool _isMoving = false;
        public bool isMoving
        {
            get { return _isMoving; }
            set { _isMoving = value; }
        }

        // timer
        public float time
        {
            get => _time;
            set => _time = value;
        }
        private float _time = 20;

        public int currentPlayer 
        { 
            get => _currentPlayer; 
            set => _currentPlayer = value;
        }
        private int _currentPlayer;

        public int maxPlayer { get => _maxPlayer; }
        private int _maxPlayer;

        private GameSceneUI gameSceneUI;

        private void Start()
        {
            gameSceneUI = GameObject.FindObjectOfType<GameSceneUI>();

            // ���� �ο� ����
            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 6:
                    _maxPlayer = 2;
                    gameSceneUI.person = maxPlayer;
                    break;
                case 7:
                    _maxPlayer = 2;
                    gameSceneUI.person = maxPlayer;
                    break;
                case 8:
                    _maxPlayer = 2;
                    gameSceneUI.person = maxPlayer;
                    break;
                case 9:
                    _maxPlayer = 1;
                    gameSceneUI.person = maxPlayer;
                    break;
            }

            SceneLoaded();
            ResetList();
        }

        private void Update()
        {
            if (_isMoving == true && SceneManager.GetActiveScene().buildIndex < 9)
            {
                // 2�ο� �ʿ����� �ð��Ȱ���
                _time -= Time.deltaTime;
            }

            if((_time <= 0 && _isFinished == false) || (_isFinished == false && _maxPlayer == _currentPlayer))
            {
                ChooseFinalWinner();
                _isFinished = true;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Scene loaded: " + scene.name);

            foreach (var photonView in FindObjectsOfType<PhotonView>())
            {
                if (photonView.IsMine)
                {
                    photonView.RequestOwnership();
                }
                else
                {
                    photonView.TransferOwnership(photonView.OwnerActorNr);
                }
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }


        void SceneLoaded()
        {
            if (SceneManager.GetActiveScene().buildIndex >= 6)
            {
                // �÷��̾��� �� �ε� ���¸� CustomProperties�� ����
                Hashtable props = new Hashtable
                {
                    { "SceneLoaded", true },
                    { "IsGoaledIn", false }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            // ����� CustomProperties Ȯ��
            if (changedProps.ContainsKey("SceneLoaded"))
            {
                // SceneLoaded�� ����Ǿ��� ���� RPC ȣ��
                if (PhotonNetwork.IsMasterClient)
                {
                    // ��� �÷��̾ ���� �ε��ߴ��� Ȯ��
                    if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("SceneLoaded") && (bool)player.CustomProperties["SceneLoaded"]))
                    {
                        // �̹� ������ ���۵��� �ʾҴ��� Ȯ��
                        if (!isGameStarted)
                        {
                            isGameStarted = true;
                            double startTime = PhotonNetwork.Time + 1f;
                            photonView.RPC("StartGame", RpcTarget.All, startTime);
                        }
                    }
                }
            }

            if (changedProps.ContainsKey("IsGoaledIn"))
            {
                if (targetPlayer.CustomProperties.TryGetValue("IsGoaledIn", out bool debugBool))
                {
                    Debug.Log($"{targetPlayer}'s isGoaledIn is " + debugBool);
                }
            }
        }

        public void OnGoaledInCheck(bool isCheck)
        {
            // ������Ƽ�� ����
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsGoaledIn", isCheck } });
            
            // ���� ��� ���� ����
            if(isCheck)
            {
                if (!_isFirstGoalIned)
                {
                    MixScene(SceneManager.GetActiveScene().buildIndex);
                    _isFirstGoalIned = true;
                }

                WinnerCountUpdate();
                //gameSceneUI.winner++;
            }
        }

        private void WinnerCountUpdate()
        {
            photonView.RPC("WinnerCount", RpcTarget.AllBuffered);
        }

        public void ChooseFinalWinner()
        {
            //photonView.RPC("RoundEnd", RpcTarget.AllBuffered);
            RoundEnd();
        }

        #region Scene
        private void ResetList()
        {
            //random�ϰ� �ҷ��� �� �ѹ�
            _round2List = new List<int>() { 7, 8 };
            _round3List = new List<int>() { 9 };
        }

        public void MixScene(int sceneNum)
        {
            List<int> list = new List<int>();
            int count = 0;
            switch (sceneNum)
            {
                case 6:
                    count = _round2List.Count;
                    for (int i = 0; i < count; i++)
                    {
                        int rand = Random.Range(0, _round2List.Count);
                        list.Add(_round2List[rand]);
                        _round2List.RemoveAt(rand);
                    }
                    _round2List = list;
                    _nextSceneNum = _round2List[0];

                    //SceneManager.LoadScene(_round2List[0]);
                    _round2List.RemoveAt(0);
                    break;
                case 7:
                case 8:
                    count = _round3List.Count;
                    for (int i = 0; i < count; i++)
                    {
                        int rand = Random.Range(0, _round3List.Count);
                        list.Add(_round3List[rand]);
                        _round3List.RemoveAt(rand);
                    }
                    _round3List = list;
                    _nextSceneNum = _round3List[0];

                    //SceneManager.LoadScene(_round2List[0]);
                    _round3List.RemoveAt(0);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region RPC
        [PunRPC]
        void StartGame(double startTime)
        {
            StartCoroutine(StartCountdown(startTime));
        }

        [PunRPC]
        private void SceneLoad(int sceneNum)
        {
            // ����-������ ĳ���;ȸ¾Ƽ�
            PhotonNetwork.LoadLevel(sceneNum);
        }

        [PunRPC]
        private void RoundEnd()
        {
            StartCoroutine(C_RoundEnd());
        }

        [PunRPC]
        private void WinnerCount()
        {
            gameSceneUI.winner++;
        }
        #endregion

        #region Coroutine
        private IEnumerator C_RoundEnd()
        {
            yield return new WaitForSeconds(5f);

            if(PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("IsGoaledIn"))
            {
                if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("IsGoaledIn", out bool isCheck))
                {
                    if(isCheck)
                    {
                        yield return new WaitForSeconds(.5f);

                        // ��¼��� ���� ���
                        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has Finish race, IsGoaledIn = " + isCheck);
                        if (SceneManager.GetActiveScene().name == "TestMap")
                        {
                            Debug.Log("���ߴ��");
                            // 2�θʿ����� ����(�ݶ��̴�������)�� �ְ� LOSE
                            StartCoroutine(LeaveRoomAndLoadScene("LoseCL"));
                            yield break;
                        }
                        // 2�θ��� �ƴϸ鼭 ��¼��� ���� �÷��̾ �ϳ����϶�
                        else if (currentPlayer == 1)
                            StartCoroutine(LeaveRoomAndLoadScene("WinnerCL"));
                        // 2�θ��� �ƴϸ鼭 2�� �̻� ��¼��� ������ ��
                        else if (PhotonNetwork.IsMasterClient)
                            photonView.RPC("SceneLoad", RpcTarget.AllBuffered, _nextSceneNum);    
                    }
                    else 
                    {
                        // ��¼��� ������ ���� ���
                        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has not Finish race, IsGoaledIn = " + isCheck);
                        if (SceneManager.GetActiveScene().name == "TestMap")
                        {
                            Debug.Log("�¸��ߴ��");
                            // 2�θʿ����� ���� ���� (�ݶ��̴��� �Ⱥε���)�ְ� ���
                            StartCoroutine(LeaveRoomAndLoadScene("WinnerCL"));
                            yield break;
                        }

                        // ��¼��� ������ ���� �÷��̾���� LOSE ó��
                        StartCoroutine(LeaveRoomAndLoadScene("LoseCL"));
                    }
                }
            }
        }

        IEnumerator StartCountdown(double startTime)
        {
            double timeRemaining = startTime - PhotonNetwork.Time;
            while (timeRemaining > 0)
            {
                yield return new WaitForEndOfFrame();
                timeRemaining = startTime - PhotonNetwork.Time;
            }
            // ���� ���� ������ ����
            if (gameSceneUI != null)
            {
                gameSceneUI.StartCoroutine("GameStart");
            }
        }

        private IEnumerator LeaveRoomAndLoadScene(string sceneName)
        {
            Debug.Log("������ڷ�ƾ�̷���");
            yield return null;
            PhotonNetwork.AutomaticallySyncScene = false;    // ������(ȣ��Ʈ)�� ���� �ѱ�� Ŭ���̾�Ʈ�鵵 ���� �Ѿ
            SceneManager.LoadScene(sceneName);
        }
        #endregion
    }
}
