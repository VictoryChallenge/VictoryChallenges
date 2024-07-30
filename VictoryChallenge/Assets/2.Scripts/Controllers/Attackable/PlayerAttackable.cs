using UnityEngine;
using Photon.Pun;
using VictoryChallenge.Controllers.Player;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.Controllers.Attackable
{
    public class PlayerAttackable : MonoBehaviour
    {
        private bool _isCollisionable;
        private int _num = 0;

        private void Start()
        {
        }

        private void Update()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                //_isCollisionable = true;
                //Debug.Log("isAttacking : " + this.gameObject.GetComponent<CharacterController>().isAttacking);
                // Attack Check
                //if (this.gameObject.GetComponent<CharacterController>().isAttacking)
                //{
                //    other.gameObject.GetComponent<CharacterController>().isDizzy = true;
                // 중복 hit체크 방지
                //if (other.gameObject.GetComponent<CharacterController>().isHit == false)
                //{
                //    // 맞은 캐릭터의 hit 애니메이션 조건 = true 설정
                //    other.gameObject.GetComponent<PhotonView>().RPC("HitCheckRPC", RpcTarget.All, true);
                //}
                //}
                //if (this.gameObject.GetComponent<CharacterController>().isPush)
                //{
                //    _num++;
                //    if(!other.gameObject.GetComponent<CharacterController>().isDizzy)
                //    {
                //        Debug.Log("push : " + _num);
                //        other.gameObject.GetComponent<CharacterController>().isDizzy = true;
                //    }
                //}
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _isCollisionable = false;
                //// Attack Check
                //if (this.gameObject.GetComponent<CharacterController>().isAttacking)
                //{
                //    // 맞은 캐릭터의 hit 애니메이션 조건 = false 설정
                //    other.gameObject.GetComponent<PhotonView>().RPC("HitCheckRPC", RpcTarget.All, false);
                //}
            }
        }
    }
}
