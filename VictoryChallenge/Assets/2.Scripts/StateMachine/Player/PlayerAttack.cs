using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine.Player
{
    /// <summary>
    /// Player Attack 상태 애니메이션 
    /// </summary>
    public class PlayerAttack : StateMachineBehaviourBase
    {
        private int _attackComboStack;
        private int _attackComboStackLimit = 1;
        private float _comboStackResetTime = 0.5f;


        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
                { State.Attack, (animator) =>
                {
                    if(!Input.GetMouseButtonDown(0))
                        return false;

                    return true;
                }},
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetInteger("State", (int)State.Attack);
        
            _attackComboStack = _attackComboStack < _attackComboStackLimit ? _attackComboStack + 1 : 0;

            animator.SetInteger("AttackComboStack", _attackComboStack);

            if (controller.comboStackResetCoroutine != null)
                controller.StopCoroutine(controller.comboStackResetCoroutine);

            // stateInfo.length : 해당 애니메이션 지속 시간
            controller.comboStackResetCoroutine =
                controller.StartCoroutine(C_ResetCombo(animator, stateInfo.length));
        }

        IEnumerator C_ResetCombo(Animator animator, float clipLength)
        {
            // 다음 콤보를 잇는데 유효한 시간
            yield return new WaitForSeconds(clipLength + _comboStackResetTime);

            // 다음 콤보를 잇는데 유효한 시간이 지나면 초기화
            _attackComboStack = 0;
            animator.SetInteger("AttackComboStack", _attackComboStack);
        }
    }
}
