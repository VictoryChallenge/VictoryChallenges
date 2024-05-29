using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player KickAttack 상태 애니메이션 
    /// </summary>
    public class PlayerKickAttack : StateMachineBehaviourBase
    {
        private bool _animEnd;


        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
                { State.Idle, (animator) =>
                {
                    return _animEnd && (controller.velocity.magnitude < 0.01f);
                }},
                { State.Move, (animator) =>
                {
                    return _animEnd && (controller.velocity.magnitude > 0.01f);
                }},
            };
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                _animEnd = true;
            }
            else
            {
                _animEnd = false;
            }
        }
    }
}
