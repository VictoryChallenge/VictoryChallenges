using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Move 상태 애니메이션 
    /// </summary>
    public class PlayerMove : StateMachineBehaviourBase
    {
        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
                { State.Jump, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.Space);
                }},
                { State.Attack, (animator) =>
                {
                    return !controller.isGrabbable && Input.GetMouseButtonDown(0);
                }},
                { State.KickAttack, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.LeftControl);
                }},
                { State.Dance, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.C);
                }},
                { State.GrabStart, (animator) =>
                {
                    return controller.isGrabbable && Input.GetMouseButton(0);
                }},
                { State.Sliding, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.Z);
                }},
                { State.Hit, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.X);
                }},
            };
        }
    }
}
