using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.StateMachine;

namespace VictoryChallenge.Controllers.Player
{
    public class PlayerController : CharacterController
    {
        private float _walkSpeed = 3f;


        protected override void Start()
        {
            base.Start();

            // Idle -> Walk, Jump, Sprint, Attack1 가능한 상태
            inputCommands = new Dictionary<State, bool>()
            {
                { State.Move, false },
                //{ State.Jump, false },
                //{ State.Sprint, false },
            };
        }

        protected override void Update()
        {
            speedGain = Input.GetKey(KeyCode.LeftShift) ? 2 * _walkSpeed : _walkSpeed;
            
            base.Update();
        }
    }
}
