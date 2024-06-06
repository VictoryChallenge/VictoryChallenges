using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Dance 상태 애니메이션 
    /// </summary>
    public class PlayerDance : StateMachineBehaviourBase
    {
        private PhotonView _pv;


        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            _pv = controller.GetComponentInParent<PhotonView>();

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
                { State.Move, (animator) =>
                {
                    return (controller.velocity.magnitude > 0.01f);
                }},
            };
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if(!_pv.IsMine)
            {
                return;
            }
        }
    }
}
