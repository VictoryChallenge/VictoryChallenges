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
    public class PlayerJumpEnd : StateMachineBehaviourBase
    {
        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
                { State.Move, (animator) =>
                {
                    return animator.IsGrounded();
                }},
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetInteger("State", (int)State.JumpEnd);

            controller.GetComponent<CapsuleCollider>().center = new Vector3(0f, 1f, 0f);
        }
    }
}
