using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using Photon.Realtime;
using System;
using VictoryChallenge.Scripts.CL;
using VictoryChallenge.Scripts.HS;
using System.Collections.Generic;
using VictoryChallenge.KJ.Database;
using Photon.Pun.Demo.Cockpit;
using GSpawn;
using VictoryChallenge.KJ.Spawn;
using ExitGames.Client.Photon.StructWrapping;
using VictoryChallenge.KJ.Lobby;

namespace VictoryChallenge.KJ.Photon
{
    public class PhotonSub : MonoBehaviourPunCallbacks
    {

        [HideInInspector] public Button _button;
        [HideInInspector] public TMP_Text _text;

        [HideInInspector] public bool _isReady = false;
        [HideInInspector] public int stageNum = 3;

        // Spawn
        private List<Player> _spawnNumList = new List<Player>();

        #region Singleton
        public static PhotonSub Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        #endregion

        #region Dynamic Buttons
        public void AssignButtonAndText()
        {
            _button = GameObject.Find("GameStart").GetComponent<Button>();
            _text = GameObject.Find("ReadyOrStart").GetComponent<TextMeshProUGUI>();
        }

        public void OnSceneLoadedForAllPlayers()
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                Debug.Log("호스트 플레이어 매니저 생성");

                // PhotonNetwork.LocalPlayer.ActorNumber는 1부터 시작
                int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
                Debug.Log("ActorNum = " + playerIndex);
                Transform spawnPoint = SpawnManager.Instance.GetIndexSpawnPoint(playerIndex);
                Quaternion rotation = Quaternion.LookRotation(Vector3.forward);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), spawnPoint.position, rotation);
            }
            else if (SceneManager.GetActiveScene().buildIndex >= 6)
            {
                Debug.Log("클라 플레이어 매니저 생성");

                //Custom Property Instantiate
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PlayerNumber", out int checkNum))
                {
                    Transform spawnPoint = SpawnManager.Instance.GetIndexSpawnPoint(checkNum);
                    Quaternion rotation = Quaternion.LookRotation(Vector3.forward);
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), spawnPoint.position, rotation);
                }
            }
        }

        public override void OnJoinedRoom()                     // 로비(룸)에 들어왔을 때
        {
            _isReady = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "IsReady", _isReady } });

            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                OnSceneLoadedForAllPlayers();
            }

            if (PhotonNetwork.IsMasterClient)
            {
                _isReady = true;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsReady", _isReady } });
                Debug.Log("호스트 준비 상태");
            }
        }

        public void UpdateButtonText()
        {
            Debug.Log("호스트인지 확인 " + PhotonNetwork.IsMasterClient);

            if (PhotonNetwork.IsMasterClient)
            {
                _text.text = "Game Start";
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(OnStartClicked);
            }
            else
            {
                _text.text = "Ready";
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(OnReadyClicked);
            }
        }

        public void OnReadyClicked()
        {
            _isReady = !_isReady;
            _text.text = _isReady == true ? "UnReady" : "Ready";
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "IsReady", _isReady } });
            Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}" + (_isReady == true ? "준비완료" : "아직 준비완료 안함"));
        }

        private void CheckAllPlayersReady()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _button.interactable = AllPlayersReady();
                Debug.Log("모든 플레이어 준비 완료" + _button.interactable);

                // 플레이어 액터넘버 등록
                ResistPlayerNumber();
            }
        }

        [PunRPC]
        private void PlayerListReady()
        {
            GameObject.FindObjectOfType<PlayerList>().ReadyPlayer();
        }

        public bool AllPlayersReady()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                object isReady;
                if (player.CustomProperties.TryGetValue("IsReady", out isReady))
                {
                    if (!(bool)isReady)
                    {
                        Debug.Log(player.NickName + "이(가) 아직 준비 안됨(포톤서브)");
                        return false;
                    }
                    else
                    {
                        Debug.Log(player.NickName + "준비됨(포톤서브)");
                    }
                }
                else
                {
                    Debug.Log(player.NickName + "이(가) 아직 준비 상태가 아님(포톤서브)");
                    return false;
                }
            }
            return true;
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            if (changedProps.ContainsKey("IsReady"))
            {
                Debug.Log(targetPlayer.NickName + "Changed ready state to" + changedProps["IsReady"]);
                photonView.RPC("PlayerListReady", RpcTarget.AllBuffered);
                CheckAllPlayersReady();
            }
        }

        public void OnStartClicked()
        {
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC("PlayerListReady", RpcTarget.AllBuffered);

            if (AllPlayersReady())
            {
                Hashtable props = new Hashtable() { { "isGameStarted", true } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);

                Debug.Log("모든 플레이어가 준비됨, 게임 시작");
                PhotonNetwork.LoadLevel(stageNum);
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            if (SceneManager.GetActiveScene().buildIndex == 2)
                UpdateButtonText();

            object isReady;
            if (!newMasterClient.CustomProperties.TryGetValue("IsReady", out isReady) || !(bool)isReady)
            {
                // 준비 상태가 아니라면 준비 상태로 변경
                Hashtable props = new Hashtable { { "IsReady", true } };
                newMasterClient.SetCustomProperties(props);
                Debug.Log("새로운 방장이 준비 상태가 아니었으므로 준비 상태로 변경함");
            }
        }
        #endregion

        public void SetStageNum(int _stageNum)
        {
            stageNum = _stageNum;
            Debug.Log("스테이지 번호 설정: " + stageNum);
        }


        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);

            if (SceneManager.GetActiveScene().buildIndex == 2)
            { 
                CheckAllPlayersReady();
            }

            StartCoroutine(C_UpdatePlayerNumber());
        }

        public void ResistPlayerNumber()
        {
            // 커스텀 프로퍼티 등록
            _spawnNumList = new List<Player>(PhotonNetwork.PlayerList);

            for (int i = 0; i < _spawnNumList.Count; i++)
            {
                Hashtable props = new Hashtable { { "PlayerNumber", i } };
                _spawnNumList[i].SetCustomProperties(props);
                Debug.Log($"List {_spawnNumList[i]}'s ActorNumber = {_spawnNumList[i].ActorNumber}, PlayerNumber = {i}");
                // List = #01 "111"s ActorNumber = 1, #02 "222"s ActorNumber = 2, #03 "333"s ActorNumber = 3 ...
            }
        }

        IEnumerator C_UpdatePlayerNumber()
        {
            // 커스텀 프로퍼티 갱신
            _spawnNumList = new List<Player>(PhotonNetwork.PlayerList);

            //for (int i = 0; i < _spawnNumList.Count; i++)
            //{
            //    Debug.Log($"After Removed List! List {_spawnNumList[i]}'s ActorNumber = {_spawnNumList[i].ActorNumber}");
            //    // List = #01 "111"s ActorNumber = 1, #02 "222"s ActorNumber = 2, #03 "333"s ActorNumber = 3 ...
            //}

            _spawnNumList.Sort((p1, p2) => p1.ActorNumber.CompareTo(p2.ActorNumber));

            for (int i = 0; i < _spawnNumList.Count; i++)
            {
                Hashtable props = new Hashtable { { "PlayerNumber", i } };
                _spawnNumList[i].SetCustomProperties(props);
                //Debug.Log($"List Update! {_spawnNumList[i]}'s ActorNumber = {_spawnNumList[i].ActorNumber}, PlayerNumber = {i}");
            }

            yield return new WaitForSeconds(1f);
        }


        public override void OnLeftRoom()                   // 로비(룸)에서 떠났으면 호출
        {
            base.OnLeftRoom();
        }
    }
}
