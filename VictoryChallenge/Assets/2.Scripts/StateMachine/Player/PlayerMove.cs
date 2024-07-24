using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.ComponentExtensions;
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
                //{ State.Jump, (animator) =>
                //{
                //    return animator.IsGrounded() && Input.GetKeyDown(KeyCode.Space);
                //}},
                { State.JumpStart, (animator) =>
                {
                    return animator.IsGrounded() && Input.GetKeyDown(KeyCode.Space);
                }},
                { State.Attack, (animator) =>
                {
                    return !controller.isGrabbable && !controller.isHit && Input.GetMouseButtonDown(0);
                }},
                { State.KickAttack, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.LeftControl);
                }},
                { State.Dance, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.T);
                }},
                { State.GrabStart, (animator) =>
                {
                    return controller.isGrabbable && Input.GetMouseButton(0);
                }},
                { State.Hit, (animator) =>
                {
                    return controller.isHit;
                }},
                { State.Sliding, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.C) && controller.isKeyActive;
                }},                
                { State.Push, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.F);
                }},
                { State.Holding, (animator) =>
                {
                    return controller.isHolding;
                }},
                {    State.Slip, (animator) =>
                {
                    return controller.isSlip && !controller.isSliping;
                }},
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetInteger("State", (int)State.Move);
        }
    }
}
