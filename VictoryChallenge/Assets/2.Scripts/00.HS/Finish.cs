using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VictoryChallenge.KJ.Database;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;
using Photon.Pun;
using VictoryChallenge.Scripts.CL;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

namespace VictoryChallenge.Scripts.HS
{
    public class Finish : MonoBehaviour
    {
        private int _rankCount;
        private PhotonView _pv;
        private GameManagerCL _gameManager;
        private bool _isFinished = false;
        private GameSceneUI _gameSCeneUI;        
        private BoxCollider _boxCollider;

        void Start()
        {
            _pv = GetComponent<PhotonView>();
            _gameManager = GameObject.Find("GameCL").GetComponent<GameManagerCL>();
            _gameSCeneUI = FindObjectOfType<GameSceneUI>();
            _boxCollider = GetComponent<BoxCollider>();
        }

        private void Update()
        {
            if (_gameManager.currentPlayer == _gameManager.maxPlayer)
            {
                _isFinished = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // 카운트가 끝나기 전에 들어왔을 때
            if (_isFinished == false)
            { 
                if (_gameManager.time > 0)
                {
                    // 플레이어가 결승선에 도착했을 때
                    if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        // 플레이어가 결승선에 도착하지 않았을 때
                        if (!other.gameObject.GetComponent<CharacterController>().isFinished)
                        {
                            // 들어온 bool 값 true 체크해서 중복 입장 불가능하게 설정
                            other.gameObject.GetComponent<CharacterController>().isFinished = true;

                            // shortUID - Rest Api
                            string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;

                            if (!string.IsNullOrEmpty(userShortUID))
                            {
                                DBManager.Instance.ReadUserData(userShortUID);
                                Debug.Log($"플레이어 {userShortUID}의 데이터 읽기 성공");

                                PhotonView photonView = other.GetComponent<PhotonView>();
                                // 본인 플레이어만 true 값으로 갱신
                                if (photonView.IsMine)
                                {
                                    Debug.LogError("RPC 쏘고있음.");
                                    _gameManager.OnGoaledInCheck(true);
                                    _pv.RPC("NotifyPlayerFinish", RpcTarget.MasterClient, photonView.ViewID);
                                }
                            }
                        }
                        Debug.Log("들어온 플레이어 수 : " + _rankCount);
                    }
                }
            }
        }

        [PunRPC]
        public void NotifyPlayerFinish(int viewID)
        {
            PhotonView targetPhotonView = PhotonView.Find(viewID);
            if (targetPhotonView != null)
            {
                // 마스터 클라이언트에서만 실행
                if (PhotonNetwork.IsMasterClient)
                {
                    if (_gameManager.currentPlayer != _gameManager.maxPlayer)
                    { 
                        _rankCount++;
                        _gameManager.currentPlayer++;
                        _pv.RPC("UpdatePlayerCount", RpcTarget.All, _gameManager.currentPlayer);
                    }
                }
            }
        }

        [PunRPC]
        public void UpdatePlayerCount(int playerCount)
        {
            _gameManager.currentPlayer = playerCount;
            Debug.LogError($"{playerCount} = {_gameManager.currentPlayer} 일까나?");
        }
    }
}
