using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Idle 상태 애니메이션 
    /// </summary>
    public class PlayerIdle : StateMachineBehaviourBase
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
                { State.Jump, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.Space);
                }},
                { State.Attack, (animator) =>
                {
                    return Input.GetMouseButtonDown(0);
                }},
                { State.KickAttack, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.LeftControl);
                }},
            };
        }
    }
}
