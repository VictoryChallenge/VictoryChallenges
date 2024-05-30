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
    public class PlayerJump : StateMachineBehaviourBase
    {
        [SerializeField] private float _jumpForce = 5.0f;



        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
                { State.Move, (animator) =>
                {
                    return controller.IsGrounded()/* && (controller.velocity.magnitude > 0.01f)*/;
                }},
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.transform.position += Vector3.up * 0.2f;
            controller.velocity = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z);
            controller.GetComponent<Rigidbody>().AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }
}
