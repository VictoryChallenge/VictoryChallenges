using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Push 상태 애니메이션 
    /// </summary>
    public class PlayerPush : StateMachineBehaviourBase
    {
        private CapsuleCollider _colLeftHand;
        private CapsuleCollider _colRightHand;

        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            _colLeftHand = controller.transform.Find("root").
                          transform.Find("pelvis").
                          transform.Find("spine_01").
                          transform.Find("spine_02").
                          transform.Find("spine_03").
                          transform.Find("clavicle_l").
                          transform.Find("upperarm_l").
                          transform.Find("lowerarm_l").
                          transform.Find("hand_l").GetComponent<CapsuleCollider>();

            _colRightHand = controller.transform.Find("root").
                                      transform.Find("pelvis").
                                      transform.Find("spine_01").
                                      transform.Find("spine_02").
                                      transform.Find("spine_03").
                                      transform.Find("clavicle_r").
                                      transform.Find("upperarm_r").
                                      transform.Find("lowerarm_r").
                                      transform.Find("hand_r").GetComponent<CapsuleCollider>();

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetInteger("State", (int)State.Push);

            controller.isPush = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f)
            {
                _colLeftHand.enabled = true;
                _colRightHand.enabled = true;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            _colLeftHand.enabled = false;
            _colRightHand.enabled = false;

            controller.isPush = false;
        }
    }
}
