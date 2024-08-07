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
            // ī��Ʈ�� ������ ���� ������ ��
            if (_isFinished == false)
            { 
                if (_gameManager.time > 0)
                {
                    // �÷��̾ ��¼��� �������� ��
                    if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        // �÷��̾ ��¼��� �������� �ʾ��� ��
                        if (!other.gameObject.GetComponent<CharacterController>().isFinished)
                        {
                            // ���� bool �� true üũ�ؼ� �ߺ� ���� �Ұ����ϰ� ����
                            other.gameObject.GetComponent<CharacterController>().isFinished = true;

                            // shortUID - Rest Api
                            string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;

                            if (!string.IsNullOrEmpty(userShortUID))
                            {
                                DBManager.Instance.ReadUserData(userShortUID);
                                Debug.Log($"�÷��̾� {userShortUID}�� ������ �б� ����");

                                PhotonView photonView = other.GetComponent<PhotonView>();
                                // ���� �÷��̾ true ������ ����
                                if (photonView.IsMine)
                                {
                                    Debug.LogError("RPC �������.");
                                    _gameManager.OnGoaledInCheck(true);
                                    _pv.RPC("NotifyPlayerFinish", RpcTarget.MasterClient, photonView.ViewID);
                                }
                            }
                        }
                        Debug.Log("���� �÷��̾� �� : " + _rankCount);
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
                // ������ Ŭ���̾�Ʈ������ ����
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
            Debug.LogError($"{playerCount} = {_gameManager.currentPlayer} �ϱ?");
        }
    }
}
