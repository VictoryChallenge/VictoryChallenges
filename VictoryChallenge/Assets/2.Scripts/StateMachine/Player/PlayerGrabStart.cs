using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Grab Start 상태 애니메이션 
    /// </summary>
    public class PlayerGrabStart : StateMachineBehaviourBase
    {
        private Transform _grabTransform;


        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            _grabTransform = controller.transform.Find("root").
                                        transform.Find("pelvis").
                                        transform.Find("spine_01").
                                        transform.Find("spine_02").
                                        transform.Find("spine_03").
                                        transform.Find("neck_01").
                                        transform.Find("head").
                                        transform.Find("Grab").GetComponent<Transform>();

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
                { State.Move, (animator) =>
                {
                    return Input.GetMouseButtonUp(0) && (controller.velocity.magnitude > 0.01f);
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

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetInteger("State", (int)State.GrabStart);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
            {
                //controller.grabbableTransform.parent = _grabTransform;
                controller.grabbableCollider.enabled = false;
                controller.grabbableTransform.position = Vector3.Lerp(controller.grabbableTransform.position, _grabTransform.position, Time.deltaTime);

                //controller.grabbableTransform.position = new Vector3(controller.grabbableTransform.position.x, controller.grabbableTransform.position.y + 2.5f * Time.deltaTime, controller.grabbableTransform.position.z);
                
                controller.grabbableRigid.useGravity = false;
            }
        }
    }
}
