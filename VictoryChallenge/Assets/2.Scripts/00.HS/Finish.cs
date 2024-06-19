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
        private bool _isActive = true;
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
            // ������ ���� ������ ��
            if(_isActive)
            {
                if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    if(!other.gameObject.GetComponent<CharacterController>().isFinished)
                    {
                        _rankCount++;
                        other.gameObject.GetComponent<CharacterController>().isFinished = true;

                        // ���� �÷��̾��� shortUID -> DB�� �ִ� shortUID�� �����ؼ� jsonData�� �޾ƿ���
                        string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;
                        DatabaseManager.Instance.gameData.users[userShortUID].rank = _rankCount;
                        Debug.Log("rank : " + DatabaseManager.Instance.gameData.users[userShortUID].rank);
                        RankManager.Instance.Register(userShortUID, _rankCount);
                        RankManager.Instance.Register("aa", 3);
                        RankManager.Instance.Register("bb", 2);

                        // DB ���� ����

                        if (_rankCount == 1)
                        {
                            StartCoroutine(C_FinishCount());
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
                        string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;
                        DatabaseManager.Instance.gameData.users[userShortUID].rank = -1;
                        Debug.Log("rank : " + DatabaseManager.Instance.gameData.users[userShortUID].rank);

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
                    RankManager.Instance.SortRank();
                }
            }
        }
    }
}
