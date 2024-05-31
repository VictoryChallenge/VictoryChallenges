using VictoryChallenge.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Controllers.Player
{
    /// <summary>
    /// Player를 제어하기 위한 스크립트
    /// </summary>
    /// 
    public class CharacterController : MonoBehaviour
    {
        #region 이동
        // 키 값
        public float horizontal { get; set; }
        public float vertical { get; set; }

        // 속도
        public virtual float speedGain { get; set; }

        public Vector3 velocity
        {
            get => _velocity;
            set => _velocity = value;
        }
        private Vector3 _velocity;

        public virtual bool isReverseKey { get; set; }

        public virtual Transform camTransform { get; set; }

        private float turnSmoothVelocity;
        public float turnSmoothTime = 0.1f;
        #endregion

        #region 애니메이션
        public Dictionary<State, bool> inputCommands;
        #endregion

        #region 컴포넌트
        private Rigidbody _rigidBody;
        protected Animator _animator;
        #endregion

        #region 코루틴
        public Coroutine comboStackResetCoroutine;
        #endregion

        protected virtual void Start()
        {
            // 컴포넌트 캐싱
            _rigidBody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();

            // 애니메이션 상태머신 등록
            InitAnimatorBehaviours();
        }

        private void FixedUpdate()
        {
            ManualMove();
        }

        protected virtual void Update()
        {
            // 키 입력에 따른 이동값 설정
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            _velocity = new Vector3(horizontal, 0f, vertical).normalized * speedGain;
            
            _animator.SetFloat("Horizontal", _velocity.x);
            _animator.SetFloat("Vertical", _velocity.z);
        }

        // Player 이동 
        void ManualMove()
        {
            if(!isReverseKey)
            {
                // 이동
                float targetAngle = Mathf.Atan2(_velocity.x, _velocity.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

                Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                transform.Translate(moveDir.normalized * _velocity.magnitude * Time.deltaTime, Space.World);

                if (_velocity != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), 0.35f);
            }
            else
            {
                // 이동
                float targetAngle = Mathf.Atan2(_velocity.x, _velocity.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

                Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                transform.Translate(-moveDir.normalized * _velocity.magnitude * Time.deltaTime, Space.World);

                if (_velocity != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-moveDir), 0.35f);
            }


            //transform.position += transform.forward * _velocity.magnitude * Time.fixedDeltaTime;

            //if (_velocity != Vector3.zero)
            //    _rigidBody.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_velocity), 0.35f);
        }

        protected virtual void InitAnimatorBehaviours()
        {
            var behaviours = _animator.GetBehaviours<StateMachineBehaviourBase>();

            foreach (var behaviour in behaviours)
            {
                behaviour.Init(this);
            }
        }
    }
}
