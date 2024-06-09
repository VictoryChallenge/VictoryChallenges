using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Dizzy 상태 애니메이션 
    /// </summary>
    public class PlayerDizzy : StateMachineBehaviourBase
    {
        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
                { State.Die, (animator) =>
                {
                    return controller.isDie;
                }},
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetInteger("State", (int)State.Dizzy);
            
            controller.dizzyCount++;
            
            if(!controller.isDie) 
            {
                controller.hitCount = 0;
            }
            
            controller.isKeyActive = false;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            controller.isKeyActive = true;
        }
    }
}
