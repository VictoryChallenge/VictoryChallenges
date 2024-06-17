using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

                    if(_rankCount == 1)
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
