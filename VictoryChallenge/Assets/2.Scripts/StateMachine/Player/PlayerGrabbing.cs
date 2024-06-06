using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Grabbing 상태 애니메이션 
    /// </summary>
    public class PlayerGrabbing : StateMachineBehaviourBase
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
                { State.Throw, (animator) =>
                {
                    return Input.GetMouseButtonDown(1);
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

            animator.SetInteger("State", (int)State.Grabbing);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            //controller.grabbableTransform.parent = _grabTransform;
            controller.grabbableTransform.position = _grabTransform.position;
            controller.grabbableTransform.forward = controller.transform.forward;

            //controller.grabbableTransform.position = new Vector3(controller.grabbableTransform.position.x + controller.moveDirection.normalized.x * controller.velocity.magnitude * Time.deltaTime, 
            //                                                     controller.grabbableTransform.position.y/* + 9.8f * Time.deltaTime*/, 
            //                                                     controller.grabbableTransform.position.z + controller.moveDirection.normalized.x * controller.velocity.magnitude * Time.deltaTime);


            //float posY = controller.grabbableTransform.position.y + 5f * Time.deltaTime;
            //Vector3 newPos = controller.moveDirection.normalized * controller.velocity.magnitude * Time.deltaTime;

            //controller.grabbableTransform.position += newPos;
        }
    }
}
