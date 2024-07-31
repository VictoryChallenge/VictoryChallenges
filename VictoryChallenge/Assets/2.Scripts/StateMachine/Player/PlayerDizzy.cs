using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using VictoryChallenge.KJ.Effect;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Dizzy 상태 애니메이션 
    /// </summary>
    public class PlayerDizzy : StateMachineBehaviourBase
    {

        private PlayerEffect _playerEffect;     // 기절 이페트

        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            _playerEffect = controller.GetComponentInChildren<PlayerEffect>();        // 기절 이펙트

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetInteger("State", (int)State.Dizzy);
            
            //if(!controller.isDie) 
            //{
            //    controller.hitCount = 0;
            //}
            
            controller.isKeyActive = false;
            controller.isDizzying = true;
            Debug.Log("dizzyEnter");

            // 기절 이펙트
            if (_playerEffect != null)
            {
                _playerEffect.ActivateDizzyEffect();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            controller.velocity = Vector3.zero;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            controller.isKeyActive = true;

            controller.isDizzy = false;
            controller.isDizzying = false;

            Debug.Log("dizzyExit");

            // 기절 이펙트
            if (_playerEffect != null)
            {
                _playerEffect.DeactivateDizzyEffect();
            }
        }
    }
}
