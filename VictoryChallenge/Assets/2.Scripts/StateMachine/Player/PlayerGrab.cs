using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Grab 상태 애니메이션 
    /// </summary>
    public class PlayerGrab : StateMachineBehaviourBase
    {
        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
                { State.Move, (animator) =>
                {
                    return (controller.velocity.magnitude > 0.01f);
                }},
                //{ State.Jump, (animator) =>
                //{
                //    return Input.GetKeyDown(KeyCode.Space);
                //}},
                //{ State.Attack, (animator) =>
                //{
                //    return Input.GetMouseButtonDown(0);
                //}},
                //{ State.KickAttack, (animator) =>
                //{
                //    return Input.GetKeyDown(KeyCode.LeftControl);
                //}},
            };
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            Vector3 otherNewPos = controller.grabbableTransform.position;
            controller.grabbableTransform.position = new Vector3(controller.grabbableTransform.position.x, controller.grabbableTransform.position.y + 0.2f * Time.deltaTime, controller.grabbableTransform.position.z);
        }
    }
}
