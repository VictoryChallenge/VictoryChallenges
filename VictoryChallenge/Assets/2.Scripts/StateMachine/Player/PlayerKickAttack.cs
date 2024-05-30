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
        public override void Init(CharacterController controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>
            {
            };
        }
    }
}
