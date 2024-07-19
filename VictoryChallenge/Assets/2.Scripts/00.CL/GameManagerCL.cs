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
    // �� ��ũ��Ʈ�� ��� �÷��̾ ���� �ε��ߴ��� Ȯ���ϰ�, �� �� ������ �����ϴ� ������ �մϴ�.
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
                Debug.Log("����������[��");
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
            //                // ��¼��� ���� ���
            //                Debug.Log(list.NickName + " has Finish race, IsGoaledIn = " + isCheck);
            //                MixScene(SceneManager.GetActiveScene().buildIndex);
            //            }
            //            else
            //            {
            //                // ��¼��� ������ ���� ���
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
            //random�ϰ� �ҷ��� �� �ѹ�
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
        // ������ Ŭ���̾�Ʈ�� �� �÷��̾��� ���¸� Ȯ���ϰ�, ��� �÷��̾ ���� �ε������� ������ �����մϴ�.
        [PunRPC]
        void PlayerLoadedScene()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("zzz������Ŭ");
                // ��� �÷��̾ ���� �ε��ߴ��� Ȯ��
                if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("SceneLoaded") && (bool)player.CustomProperties["SceneLoaded"]))
                {
                    Debug.Log("�־ȳ����µ��ֿֿֿ֤Ǿ֤Ǿ֤Ǿ�");
                    // ��� �÷��̾ ���� �ε������� �˸��� RPC ȣ��
                    photonView.RPC("StartGame", RpcTarget.All);
                }
            }
        }

        [PunRPC]
        void StartGame()
        {
            // ���� ���� ������ ����
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
                            // ��¼��� ���� ���
                            Debug.Log(list.NickName + " has Finish race, IsGoaledIn = " + isCheck);
                            MixScene(SceneManager.GetActiveScene().buildIndex);
                            if (!_isFirstGoalIned)
                                _isFirstGoalIned = true;
                        }
                        else
                        {
                            // ��¼��� ������ ���� ���
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
