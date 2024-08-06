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
            // 카운트가 끝나기 전에 들어왔을 때
            if(_gameManager.time > 0)
            {
                // 플레이어가 결승선에 도착했을 때
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    // 플레이어가 결승선에 도착하지 않았을 때
                    if (!other.gameObject.GetComponent<CharacterController>().isFinished)
                    {
                        // 들어온 인원 수 증가
                        _rankCount++;
                        _gameManager.currentPlayer++;

                        // 들어온 bool 값 true 체크해서 중복 입장 불가능하게 설정
                        other.gameObject.GetComponent<CharacterController>().isFinished = true;

                        // shortUID - SDK, Photon
                        // 각자 플레이어의 shortUID 받아오기
                        //string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;

                        // 각자 플레이어의 nickName 받아오기
                        //string nickName = other.gameObject.GetComponent<CharacterController>().nickName;

                        // shortUID와 rank를 DB에 연동하기 위해 RankManager에 등록
                        //_gameManager.GetComponent<GameManagerCL>().SetRank(_rankCount);
                        //_gameManager.GetComponent<GameManagerCL>().Register(nickName, _rankCount);

                        // shortUID - Rest Api
                        string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;

                        if(!string.IsNullOrEmpty(userShortUID))
                        {
                            DBManager.Instance.ReadUserData(userShortUID);
                            Debug.Log($"플레이어 {userShortUID}의 데이터 읽기 성공");

                            PhotonView photonView = other.GetComponent<PhotonView>();
                            // 본인 플레이어만 true 값으로 갱신
                            if(photonView.IsMine)
                            {
                                _gameManager.OnGoaledInCheck(true);
                            }
                        }

                        // 한명이라도 들어오면 카운트 시작
                        //if (_rankCount == 1)
                        //{
                            //_gameManager.MixScene(SceneManager.GetActiveScene().buildIndex);
                            //_pv.RPC("StartCount", RpcTarget.AllBuffered);
                        //}
                    }
                    Debug.Log("들어온 플레이어 수 : " + _rankCount);
                }
            }
            //// 끝나고 나서 들어왔을 때
            //else
            //{
            //    if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            //    {
            //        if (!other.gameObject.GetComponent<CharacterController>().isFinished)
            //        {
            //            // 각자 플레이어의 shortUID -> DB에 있는 shortUID에 접근해서 jsonData를 받아오기
            //            //string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;


            //            //RankManager.Instance.Register(userShortUID, _rankCount);

            //            // DB 연동 순위
            //            PhotonView photonView = other.GetComponent<PhotonView>();
            //            // 본인 플레이어만 true 값으로 갱신
            //            if (photonView.IsMine)
            //            {
            //                _gameManager.OnGoaledInCheck(false);
            //            }
            //        }
            //        Debug.Log("들어온 플레이어 수 : " + _rankCount);
            //    }
            //}
        }
    }
}
