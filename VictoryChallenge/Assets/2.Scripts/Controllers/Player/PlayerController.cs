using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.StateMachine;
using VictoryChallenge.ComponentExtensions;
using GSpawn;
using Photon.Pun;

namespace VictoryChallenge.Controllers.Player
{
    public class PlayerController : CharacterController
    {
        private float _walkSpeed = 3f;

        [SerializeField] CinemachineVirtualCamera _vCamPerspective;
        private PhotonView _pv;

        protected override void Start()
        {
            camTransform = _vCamPerspective.gameObject.GetComponent<Transform>();
            _pv = GetComponent<PhotonView>();

            base.Start();

            if(!_pv.IsMine)
            {
                return;
            }

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
