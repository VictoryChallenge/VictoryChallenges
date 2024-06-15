using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Slip 상태 애니메이션 
    /// </summary>
    public class PlayerSlip : StateMachineBehaviourBase
    {
        [SerializeField] private float _slipPower = 5f;


        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetInteger("State", (int)State.Slip);
            controller.isSliping = true;
            
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            controller.transform.position += -controller.transform.forward * _slipPower * Time.deltaTime;
            
            controller.velocity = Vector3.zero;
        }
    }
}
