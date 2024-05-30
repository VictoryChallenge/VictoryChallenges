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

        [Header("Camera")]
        private bool _isChangePerspective;
        [SerializeField] private float _rotateSpeedY;
        [SerializeField] private float _rotateSpeedX;
        [SerializeField] private float _angleXMin = -8.0f;
        [SerializeField] private float _angleXMax = 45.0f;
        [SerializeField] private float _fovMin = 20.0f;
        [SerializeField] private float _fovMax = 80.0f;
        [SerializeField] private float _scrollThreshold; // 마우스 스크롤 임계점, <-> 민감도
        [SerializeField] private float _scrollSpeed; // 마우스 스크롤 스피드
        private float _mouseX;
        private float _mouseY;
        private float _mouseScroll;

        [SerializeField] CinemachineVirtualCamera _vCamFollow;
        [SerializeField] CinemachineVirtualCamera _vCamPerspective;


        protected override void Start()
        {
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
            
            _isChangePerspective = Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E) ? true : false;

            base.Update();
        }

        private void LateUpdate()
        {
            if(!_isChangePerspective)
            {
                _vCamFollow.enabled = true;
                _vCamPerspective.enabled = false;

                UpdateSight();
                UpdateZoom();
            }
            else
            {
                _vCamPerspective.enabled = true;
                _vCamFollow.enabled = false;
            }
        }

        private void UpdateSight()
        {
            _mouseX = Input.GetAxis("Mouse X");
            _mouseY = Input.GetAxis("Mouse Y");
            _mouseScroll = Input.GetAxis("Mouse ScrollWheel");

            transform.Rotate(Vector3.up, _mouseX * _rotateSpeedY, Space.World);
            _vCamFollow.Follow.Rotate(Vector3.left, _mouseY * _rotateSpeedX, Space.Self);
            _vCamFollow.Follow.localRotation
                = Quaternion.Euler(_vCamFollow.Follow.localEulerAngles.x.ClampAsNormalizedAngle(_angleXMin, _angleXMax), 0.0f, 0.0f);


        }

        private void UpdateZoom()
        {
            if (Mathf.Abs(_mouseScroll) < _scrollThreshold)
                return;

            _vCamFollow.m_Lens.FieldOfView -= _mouseScroll * _scrollSpeed;
            _vCamFollow.m_Lens.FieldOfView = Mathf.Clamp(_vCamFollow.m_Lens.FieldOfView, _fovMin, _fovMax);
        }
    }
}
