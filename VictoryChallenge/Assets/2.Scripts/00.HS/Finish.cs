using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VictoryChallenge.KJ.Database;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.Scripts.HS
{
    public class Finish : MonoBehaviour
    {
        private int _rankCount;
        private int _maxCount = 10;

        [SerializeField] public TextMeshProUGUI _countText;

        void Start()
        {
            //_countText.enabled = false;
        }

        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if(!other.gameObject.GetComponent<CharacterController>().isFinished)
                {
                    _rankCount++;
                    other.gameObject.GetComponent<CharacterController>().isFinished = true;

                    // 각자 플레이어의 shortUID -> DB에 있는 shortUID에 접근해서 jsonData를 받아오기
                    string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;
                    DatabaseManager.Instance.gameData.users[userShortUID].rank = _rankCount;
                    Debug.Log("rank : " + DatabaseManager.Instance.gameData.users[userShortUID].rank);

                    if (_rankCount == 1)
                    {
                        StartCoroutine(C_FinishCount());
                    }
                }
                Debug.Log("들어온 플레이어 수 : " + _rankCount);
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
                    _countText.text = "Game Over";
                }
            }
        }
    }
}
