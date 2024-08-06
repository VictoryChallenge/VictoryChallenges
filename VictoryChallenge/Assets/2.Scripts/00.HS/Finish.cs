using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VictoryChallenge.KJ.Database;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;
using Photon.Pun;
using VictoryChallenge.Scripts.CL;
using UnityEngine.SceneManagement;

namespace VictoryChallenge.Scripts.HS
{
    public class Finish : MonoBehaviour
    {
        private int _rankCount;
        private int _maxCount = 10;
        private PhotonView _pv;
        private GameManagerCL _gameManager;
        private GameSceneUI _gameSCeneUI;


        [SerializeField] public TextMeshProUGUI _countText;

        void Start()
        {
            _pv = GetComponent<PhotonView>();
            _gameManager = GameObject.Find("GameCL").GetComponent<GameManagerCL>();
            _gameSCeneUI = FindObjectOfType<GameSceneUI>();
        }

        private void OnTriggerEnter(Collider other)
        {
            // ī��Ʈ�� ������ ���� ������ ��
            if(_gameManager.time > 0)
            {
                // �÷��̾ ��¼��� �������� ��
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    // �÷��̾ ��¼��� �������� �ʾ��� ��
                    if (!other.gameObject.GetComponent<CharacterController>().isFinished)
                    {
                        // ���� �ο� �� ����
                        _rankCount++;
                        _gameManager.currentPlayer++;

                        // ���� bool �� true üũ�ؼ� �ߺ� ���� �Ұ����ϰ� ����
                        other.gameObject.GetComponent<CharacterController>().isFinished = true;

                        // shortUID - SDK, Photon
                        // ���� �÷��̾��� shortUID �޾ƿ���
                        //string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;

                        // ���� �÷��̾��� nickName �޾ƿ���
                        //string nickName = other.gameObject.GetComponent<CharacterController>().nickName;

                        // shortUID�� rank�� DB�� �����ϱ� ���� RankManager�� ���
                        //_gameManager.GetComponent<GameManagerCL>().SetRank(_rankCount);
                        //_gameManager.GetComponent<GameManagerCL>().Register(nickName, _rankCount);

                        // shortUID - Rest Api
                        string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;

                        if(!string.IsNullOrEmpty(userShortUID))
                        {
                            DBManager.Instance.ReadUserData(userShortUID);
                            Debug.Log($"�÷��̾� {userShortUID}�� ������ �б� ����");

                            PhotonView photonView = other.GetComponent<PhotonView>();
                            // ���� �÷��̾ true ������ ����
                            if(photonView.IsMine)
                            {
                                _gameManager.OnGoaledInCheck(true);
                            }
                        }

                        // �Ѹ��̶� ������ ī��Ʈ ����
                        //if (_rankCount == 1)
                        //{
                            //_gameManager.MixScene(SceneManager.GetActiveScene().buildIndex);
                            //_pv.RPC("StartCount", RpcTarget.AllBuffered);
                        //}
                    }
                    Debug.Log("���� �÷��̾� �� : " + _rankCount);
                }
            }
            //// ������ ���� ������ ��
            //else
            //{
            //    if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            //    {
            //        if (!other.gameObject.GetComponent<CharacterController>().isFinished)
            //        {
            //            // ���� �÷��̾��� shortUID -> DB�� �ִ� shortUID�� �����ؼ� jsonData�� �޾ƿ���
            //            //string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;


            //            //RankManager.Instance.Register(userShortUID, _rankCount);

            //            // DB ���� ����
            //            PhotonView photonView = other.GetComponent<PhotonView>();
            //            // ���� �÷��̾ true ������ ����
            //            if (photonView.IsMine)
            //            {
            //                _gameManager.OnGoaledInCheck(false);
            //            }
            //        }
            //        Debug.Log("���� �÷��̾� �� : " + _rankCount);
            //    }
            //}
        }
    }
}
