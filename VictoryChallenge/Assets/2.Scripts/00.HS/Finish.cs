using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VictoryChallenge.KJ.Database;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;
using Photon.Pun;
using VictoryChallenge.Scripts.CL;

namespace VictoryChallenge.Scripts.HS
{
    public class Finish : MonoBehaviour
    {
        private bool _isActive = true;
        private int _rankCount;
        private int _maxCount = 10;
        private PhotonView _pv;
        private GameObject _gameManager;


        [SerializeField] public TextMeshProUGUI _countText;

        void Start()
        {
            _pv = GetComponent<PhotonView>();
            _gameManager = GameObject.Find("GameCL");
        }

        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            // ī��Ʈ�� ������ ���� ������ ��
            if (_isActive)
            {
                // �÷��̾ ��¼��� �������� ��
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    // �÷��̾ ��¼��� �������� �ʾ��� ��
                    if (!other.gameObject.GetComponent<CharacterController>().isFinished)
                    {
                        // ���� ������� ���� ����
                        _rankCount++;

                        // ���� bool �� true üũ�ؼ� �ߺ� ���� �Ұ����ϰ� ����
                        other.gameObject.GetComponent<CharacterController>().isFinished = true;

                        // shortUID - SDK, Photon
                        // ���� �÷��̾��� shortUID �޾ƿ���
                        //string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;

                        // ���� �÷��̾��� nickName �޾ƿ���
                        //string nickName = other.gameObject.GetComponent<CharacterController>().nickName;

                        // shortUID�� rank�� DB�� �����ϱ� ���� RankManager�� ���
                        //PlayersDataManager.Instance.SetRank(userShortUID, _rankCount);
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
                                _gameManager.GetComponent<GameManagerCL>().OnGoaledInCheck();
                            }
                        }

                        // �Ѹ��̶� ������ ī��Ʈ ����
                        if (_rankCount == 1)
                        {
                            _pv.RPC("StartCount", RpcTarget.AllBuffered);
                        }
                    }
                    Debug.Log("���� �÷��̾� �� : " + _rankCount);
                }
            }
            // ������ ���� ������ ��
            else
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    if (!other.gameObject.GetComponent<CharacterController>().isFinished)
                    {
                        // ���� �÷��̾��� shortUID -> DB�� �ִ� shortUID�� �����ؼ� jsonData�� �޾ƿ���
                        //string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;


                        //RankManager.Instance.Register(userShortUID, _rankCount);

                        // DB ���� ����
                    }
                    Debug.Log("���� �÷��̾� �� : " + _rankCount);
                }
            }
        }

        IEnumerator C_FinishCount()
        {
            bool isCheck = true;
            _countText.enabled = true;
            _countText.text = _maxCount.ToString();

            while (isCheck)
            {
                yield return new WaitForSeconds(1f);
                _maxCount--;
                _countText.text = _maxCount.ToString();

                if (_maxCount == 0)
                {
                    isCheck = false;
                    _isActive = false;
                    _countText.text = "Game Over";
                    _gameManager.GetComponent<GameManagerCL>().ChooseFinalWinner();
                }
            }
        }

        [PunRPC]
        public void StartCount()
        {
            StartCoroutine(C_FinishCount());
        }

    }
}
