using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Throw 상태 애니메이션 
    /// </summary>
    public class PlayerThrow : StateMachineBehaviourBase
    {
        [SerializeField] private float throwPower = 5f;
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
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetInteger("State", (int)State.Throw);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f)
            {
                //controller.grabbableTransform.parent = null;

                Vector3 newPos = -_grabTransform.up * throwPower * Time.deltaTime; 

                controller.grabbableTransform.position += newPos;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            controller.grabbableCollider.enabled = true;
            controller.grabbableRigid.useGravity = true;
        }
    }
}
