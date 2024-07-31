using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player KickAttack 상태 애니메이션 
    /// </summary>
    public class PlayerKickAttack : StateMachineBehaviourBase
    {
        private CapsuleCollider _colLeftFoot;
        private CapsuleCollider _colRightFoot;


        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            _colLeftFoot = controller.transform.Find("root").
                                      transform.Find("pelvis").
                                      transform.Find("thigh_l").
                                      transform.Find("calf_l").
                                      transform.Find("foot_l").
                                      transform.Find("ball_l").GetComponent<CapsuleCollider>();

            _colRightFoot = controller.transform.Find("root").
                                      transform.Find("pelvis").
                                      transform.Find("thigh_r").
                                      transform.Find("calf_r").
                                      transform.Find("foot_r").
                                      transform.Find("ball_r").GetComponent<CapsuleCollider>();

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetInteger("State", (int)State.KickAttack);
            _colLeftFoot.enabled = true;
            _colRightFoot.enabled = true;

            controller.isKick = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            controller.transform.position += controller.transform.forward * 3f * Time.deltaTime;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            _colLeftFoot.enabled = false;
            _colRightFoot.enabled = false;

            controller.isKick = false;
        }
    }
}
