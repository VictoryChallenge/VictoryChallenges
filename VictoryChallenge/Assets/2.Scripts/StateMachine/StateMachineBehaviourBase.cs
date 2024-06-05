using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.StateMachine
{
    public class StateMachineBehaviourBase : StateMachineBehaviour
    {
        public Dictionary<State, Func<Animator, bool>> transitions;
        protected CharacterController controller;
        private bool _inTransition = false;


        public virtual void Init(CharacterController controller)
        {
            this.controller = controller;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            // 상태 진입을 제대로 했으니 IsDirty 초기화

            animator.SetBool("IsDirty", false);
            _inTransition = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            foreach (var transition in transitions)
            {
                if (ChangeState(animator, transition.Key))
                {
                    _inTransition = true;
                    break;
                }
            }
        }

        protected bool ChangeState(Animator animator, State newState)
        {
            // 상태가 전이 중인 상태인지 체크
            if (_inTransition)
                return false;

            // 해당 State가 Dictionary Key에 있는지 체크
            if (transitions.ContainsKey(newState) == false)
                return false;

            // 해당 State의 transition 조건 체크
            if (transitions[newState].Invoke(animator) == false)
                return false;

            //if (controller.GetComponent<PhotonView>() != null)
            //{
            //    Debug.Log("호출");
            //    controller.GetComponent<PhotonView>().RPC("ChangeStateClientRpc", RpcTarget.All, newState);
            //}

            animator.SetInteger("State", (int)newState);
            animator.SetBool("IsDirty", true);
            return true;
        }
    }
}
