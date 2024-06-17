using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.ComponentExtensions;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Jump 상태 애니메이션 
    /// </summary>
    public class PlayerJumping : StateMachineBehaviourBase
    {
        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
                { State.JumpEnd, (animator) =>
                {
                    return controller.velocity.y <= 0;
                }},
                { State.Sliding, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.C);
                }},
                { State.Slip, (animator) =>
                {
                    return controller.isSlip && !controller.isSliping;
                }},
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            
            animator.SetInteger("State", (int)State.Jumping);
        }
    }
}
