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
        private int _nextSceneNum;
        private List<int> _round2List;

        // PlayerActive
        private bool _isMoving = false;
        public bool isMoving
        {
            get { return _isMoving; }
            set { _isMoving = value; }
        }

        // ?
        private bool _isGameStarted;


        // timer
        public float time
        {
            get => _time;
            set => _time = value;
        }
        private float _time = 100;

        public int currentPlayer 
        { 
            get => _currentPlayer; 
            set => _currentPlayer = value;
        }
        private int _currentPlayer;

        public int maxPlayer { get => _maxPlayer; }
        private int _maxPlayer = 2;


        private void Start()
        {
            OnSceneLoaded();

            ResetList();
        }

        private void Update()
        {
            if (_isMoving == true && time >= 0)
            {
                time -= Time.deltaTime;
            }

            if(time <= 0 || maxPlayer == currentPlayer)
            {
                ChooseFinalWinner();
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
                photonView.RPC("PlayerLoadedScene", RpcTarget.All);
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
            _round2List = new List<int>() { 5, 6 };
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
        // 마스터 클라이언트가 각 플레이어의 상태를 확인하고, 모든 플레이어가 씬을 로드했으면 게임을 시작합니다.
        [PunRPC]
        void PlayerLoadedScene()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("zzz마스터클");
                // 모든 플레이어가 씬을 로드했는지 확인
                if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("SceneLoaded") && (bool)player.CustomProperties["SceneLoaded"]))
                {
                    Debug.Log("왜안나오는데왜왜왜왜ㅗ애ㅗ애ㅗ애");
                    // 모든 플레이어가 씬을 로드했음을 알리는 RPC 호출
                    photonView.RPC("StartGame", RpcTarget.All);
                }
            }
        }

        [PunRPC]
        void StartGame()
        {
            // 게임 시작 로직을 실행
            GameSceneUI gameSceneUI = FindObjectOfType<GameSceneUI>();
            if (gameSceneUI != null)
            {
                gameSceneUI.StartCoroutine("GameStart");
            }
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
            yield return new WaitForSeconds(2f);

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
                            SceneManager.LoadScene(1);
                        }
                    }
                }
            }
        }
    }
    #endregion
}
