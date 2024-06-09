using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.ComponentExtensions;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Hit 상태 애니메이션 
    /// </summary>
    public class PlayerHit : StateMachineBehaviourBase
    {
        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
                { State.Dizzy, (animator) =>
                {
                    return !controller.isDie && controller.isDizzy;
                }},
                { State.Die, (animator) =>
                {
                    return controller.isDie;
                }},
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetInteger("State", (int)State.Hit);

            controller.hitCount++;
        }
    }
}
