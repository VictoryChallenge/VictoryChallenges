using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.StateMachine;
using VictoryChallenge.ComponentExtensions;
using GSpawn;

namespace VictoryChallenge.Controllers.Player
{
    public class PlayerController : CharacterController
    {
        private float _walkSpeed = 3f;

        [SerializeField] CinemachineVirtualCamera _vCamPerspective;


        protected override void Start()
        {
            camTransform = _vCamPerspective.gameObject.GetComponent<Transform>();

            base.Start();

            // Idle -> Walk, Jump, Attack, KickAttack 가능한 상태
            inputCommands = new Dictionary<State, bool>()
            {
                { State.Move, false },
                { State.Jump, false },
                { State.Attack, false },
                { State.KickAttack, false },
            };
        }

        protected override void Update()
        {
            speedGain = Input.GetKey(KeyCode.LeftShift) ? 2 * _walkSpeed : _walkSpeed;
            
            base.Update();
        }
    }
}