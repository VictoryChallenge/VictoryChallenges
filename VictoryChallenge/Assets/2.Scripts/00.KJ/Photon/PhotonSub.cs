using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Realtime;

namespace VictoryChallenge.KJ.Photon
{
    public class PhotonSub : MonoBehaviourPunCallbacks
    {

        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;

        private bool _isReady = false;
        //private bool _isControllerCreated = false;

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
            _text = GameObject.Find("ReadyOrStart").GetComponent<TMP_Text>();
        }

        public void OnSceneLoadedForAllPlayers()
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                Debug.Log("클라이언트 플레이어 매니저 생성");
                //if (!_isControllerCreated)
                //{
                //    GameObject playerManager = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
                //    DontDestroyOnLoad(playerManager);
                //    playerManager.GetComponent<PlayerManager>().CreateController();
                //    _isControllerCreated = true;
                //}

                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            }
            else if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            }
        }

        public override void OnJoinedRoom()                     // 로비(룸)에 들어왔을 때
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                OnSceneLoadedForAllPlayers();
            }

            StartCoroutine(UpdateButtonTextWithDelay());

            if (PhotonNetwork.IsMasterClient)
            {
                _isReady = true;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsReady", _isReady } });
                Debug.Log("호스트 준비 상태");
            }

            Debug.Log("유저 이름 " + PhotonNetwork.NickName);
        }

        private IEnumerator UpdateButtonTextWithDelay()
        {
            yield return new WaitForSeconds(0.1f);
            AssignButtonAndText();
            UpdateButtonText();
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
            _text.text = _isReady ? "UnReady" : "Ready";
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsReady", _isReady } });
            Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}" + (_isReady ? "준비완료" : "아직 준비완료 안함"));
            CheckAllPlayersReady();
        }

        private void CheckAllPlayersReady()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _button.interactable = AllPlayersReady();
                Debug.Log("모든 플레이어 준비 완료" + _button.interactable);
            }
        }

        private bool AllPlayersReady()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                object isReady;
                if (player.CustomProperties.TryGetValue("IsReady", out isReady))
                {
                    if (!(bool)isReady)
                    {
                        Debug.Log(player.NickName + "이(가) 아직 준비 안됨");
                        return false;
                    }
                    else
                    {
                        Debug.Log(player.NickName + "준비됨");
                    }
                }
                else
                {
                    Debug.Log(player.NickName + "이(가) 아직 준비 상태가 아님");
                    return false;
                }
            }
            return true;
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            if (changedProps.ContainsKey("isReady"))
            {
                Debug.Log(targetPlayer.NickName + "Changed ready state to" + changedProps["IsReady"]);
                CheckAllPlayersReady();
            }
        }

        public void OnStartClicked()
        {
            if (PhotonNetwork.IsMasterClient && AllPlayersReady())
            {
                Debug.Log("모든 플레이어가 준비됨, 게임 시작");
                PhotonNetwork.LoadLevel(2);
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            OnStartClicked();
        }
        #endregion
    }
}
