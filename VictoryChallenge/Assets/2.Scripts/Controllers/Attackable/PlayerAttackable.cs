using UnityEngine;
using Photon.Pun;
using VictoryChallenge.Controllers.Player;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.Controllers.Attackable
{
    public class PlayerAttackable : MonoBehaviour
    {
        private CharacterController _characterController;


        private void Start()
        {
            _characterController = GetComponentInParent<CharacterController>();
        }

        private void Update()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            // Attack Check
            if (_characterController.isAttacking)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    // 중복 hit체크 방지
                    if(other.gameObject.GetComponent<CharacterController>().isHit == false)
                    {
                        // 맞은 캐릭터의 hit 애니메이션 조건 = true 설정
                        other.gameObject.GetComponent<PhotonView>().RPC("HitCheckRPC", RpcTarget.All, true);
                    }
                }
            }
        }
    }
}
