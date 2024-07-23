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


namespace VictoryChallenge.Scripts.CL
{
    // 이 스크립트는 모든 플레이어가 씬을 로드했는지 확인하고, 그 후 게임을 시작하는 역할을 합니다.
    public class GameManagerCL : MonoBehaviourPunCallbacks
    {
        // random scene list
        private bool _isFirstGoalIned;
        private bool _isFinished = false;
        private int _nextSceneNum;
        private List<int> _round2List;

        // 커스텀 프로퍼티 한번만 불러오기 위한
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
        private int _maxPlayer = 2;

        private GameSceneUI gameSceneUI;

        private void Start()
        {
            gameSceneUI = GameObject.FindObjectOfType<GameSceneUI>();

            OnSceneLoaded();
            ResetList();
        }

        private void Update()
        {
            if (_isMoving == true)
            {
                time -= Time.deltaTime;
            }

            if((time <= 0 && _isFinished == false) || (_isFinished == false && maxPlayer == currentPlayer))
            {
                ChooseFinalWinner();
                _isFinished = true;
            }
        }

        void OnSceneLoaded()
        {
            if (SceneManager.GetActiveScene().buildIndex >= 3 && SceneManager.GetActiveScene().buildIndex != 4)
            {
                Debug.Log("ㅇㅇㅇㅋ제[발");
                // 플레이어의 씬 로드 상태를 CustomProperties에 설정
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
            // 변경된 CustomProperties 확인
            if (changedProps.ContainsKey("SceneLoaded"))
            {
                // SceneLoaded가 변경되었을 때만 RPC 호출
                if (PhotonNetwork.IsMasterClient)
                {
                    // 모든 플레이어가 씬을 로드했는지 확인
                    if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("SceneLoaded") && (bool)player.CustomProperties["SceneLoaded"]))
                    {
                        // 이미 게임이 시작되지 않았는지 확인
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
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsGoaledIn", isCheck } });
        }

        public void ChooseFinalWinner()
        {
            photonView.RPC("RoundEnd", RpcTarget.AllBuffered);

            //foreach(var list in PhotonNetwork.PlayerList)
            //{
            //    if(list.CustomProperties.ContainsKey("IsGoaledIn"))
            //    {
            //        if(list.CustomProperties.TryGetValue("IsGoaledIn", out bool isCheck))
            //        {
            //            if(isCheck)
            //            {
            //                // 결승선에 들어온 경우
            //                Debug.Log(list.NickName + " has Finish race, IsGoaledIn = " + isCheck);
            //                MixScene(SceneManager.GetActiveScene().buildIndex);
            //            }
            //            else
            //            {
            //                // 결승선에 들어오지 못한 경우
            //                Debug.Log(list.NickName + " has not Finish race, IsGoaledIn = " + isCheck);
            //                RoomMananger.Instance.LeaveRoom();
            //                SceneManager.LoadScene(1);
            //            }
            //        }
            //    }
            //}
        }

        #region Scene
        private void ResetList()
        {
            //random하게 불러올 씬 넘버
            _round2List = new List<int>() { /*5*/ 6 };
        }

        public void MixScene(int sceneNum)
        {
            List<int> list = new List<int>();
            int count = 0;
            switch (sceneNum)
            {
                case 3:
                    count = _round2List.Count;
                    for (int i = 0; i < count; i++)
                    {
                        int rand = Random.Range(0, _round2List.Count);
                        list.Add(_round2List[rand]);
                        _round2List.RemoveAt(rand);
                    }
                    _round2List = list;
                    if(!_isFirstGoalIned)
                    {
                        photonView.RPC("SceneLoad", RpcTarget.AllBuffered, _round2List[0]);
                        _nextSceneNum = _round2List[0];
                    }
                    else
                    {
                        photonView.RPC("SceneLoad", RpcTarget.AllBuffered, _nextSceneNum);
                    }
                    //SceneManager.LoadScene(_round2List[0]);
                    _round2List.RemoveAt(0);
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
            SceneManager.LoadScene(sceneNum);
        }

        [PunRPC]
        private void RoundEnd()
        {
            StartCoroutine(C_RoundEnd());
        }

        private IEnumerator C_RoundEnd()
        {
            yield return new WaitForSeconds(5f);

            foreach(var list in PhotonNetwork.PlayerList)
            {
                if(list.CustomProperties.ContainsKey("IsGoaledIn"))
                {
                    if(list.CustomProperties.TryGetValue("IsGoaledIn", out bool isCheck))
                    {
                        if(isCheck)
                        {
                            // 결승선에 들어온 경우
                            Debug.Log(list.NickName + " has Finish race, IsGoaledIn = " + isCheck);
                            MixScene(SceneManager.GetActiveScene().buildIndex);
                            if (!_isFirstGoalIned)
                                _isFirstGoalIned = true;
                        }
                        else
                        {
                            // 결승선에 들어오지 못한 경우
                            Debug.Log(list.NickName + " has not Finish race, IsGoaledIn = " + isCheck);
                            RoomMananger.Instance.LeaveRoom();
                            //SceneManager.LoadScene(1);
                        }
                    }
                }
            }
        }

        IEnumerator StartCountdown(double startTime)
        {
            double timeRemaining = startTime - PhotonNetwork.Time;
            while (timeRemaining > 0)
            {
                Debug.Log("Time remaining: " + timeRemaining);
                yield return new WaitForEndOfFrame();
                timeRemaining = startTime - PhotonNetwork.Time;
            }
            // 게임 시작 로직을 실행
            if (gameSceneUI != null)
            {
                gameSceneUI.StartCoroutine("GameStart");
            }
        }
    }
    #endregion
}
