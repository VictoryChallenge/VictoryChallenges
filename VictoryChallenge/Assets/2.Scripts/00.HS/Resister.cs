using UnityEngine;
using VictoryChallenge.Controllers.Player;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.Scripts.HS
{
    public class Resister : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // 플레이어가 시작할 때 데이터 등록
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // 플레이어가 결승선에 도착하지 않았을 때
                if (!other.gameObject.GetComponent<CharacterController>().isFinished)
                {
                    //// 각자 플레이어의 shortUID 받아오기
                    //string userShortUID = other.gameObject.GetComponent<CharacterController>().shortUID;
                    //PlayerController[] players = FindObjectsOfType<PlayerController>();
                    //foreach (PlayerController player in players)
                    //{
                    //    Debug.Log("userShortUID => " + player.shortUID);
                    //    RankManager.Instance.Register(player.shortUID, 0);
                    //}

                    // shortUID와 rank를 DB에 연동하기 위해 RankManager에 등록
                }
            }
        }
    }
}
