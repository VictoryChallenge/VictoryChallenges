using VictoryChallenge.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

namespace VictoryChallenge.Controllers.Player
{
    /// <summary>
    /// Player를 제어하기 위한 스크립트
    /// </summary>
    /// 
    public class CharacterController : MonoBehaviourPunCallbacks , IPunObservable
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

        public bool isKeyActive 
        { 
            get => _isKeyActive; 
            set => _isKeyActive = value; 
        }

        private bool _isKeyActive = true;

        // 카메라 회전
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

        #region 상호 작용
        // Grab
        public virtual bool isGrabbable { get; set; }
        public virtual Transform grabbableTransform { get; set; }
        public virtual Rigidbody grabbableRigid { get; set; }
        public virtual CapsuleCollider grabbableCollider { get; set; }

        // Hit
        public virtual bool isHit { get; set; }
        public virtual bool isDizzy { get; set; }
        public virtual bool isDie { get; set; }

        public virtual int hitCount { get; set; }
        public virtual int dizzyCount { get; set; }

        // Object
        public virtual bool isReverseKey { get; set; }
        #endregion

        #region 포톤
        private PhotonView _pv;

        #endregion
        private void Awake()
        {
            // 포톤뷰 캐싱
            _pv = GetComponent<PhotonView>();

            // 컴포넌트 캐싱
            _rigidBody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            // 애니메이션 상태머신 등록
            InitAnimatorBehaviours();
            
            //if (!_pv.IsMine)
            //{
            //    return;
            //}
        }

        private void FixedUpdate()
        {
            ManualMove();
        }

        protected virtual void Update()
        {
            if (_pv.IsMine)
            {
                // 키 입력에 따른 이동값 설정
                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");

                if (_isKeyActive)
                {
                    _velocity = new Vector3(horizontal, 0f, vertical).normalized * speedGain;
                }

                _animator.SetFloat("Horizontal", _velocity.x);
                _animator.SetFloat("Vertical", _velocity.z);

                if (dizzyCount > 2)
                {
                    isDie = true;
                }

                if (!isDie)
                {
                    if (hitCount > 2)
                    {
                        isDizzy = true;
                    }
                    else
                    {
                        isDizzy = false;
                    }
                }
                else
                {
                    isDizzy = false;
                }
            }
        }

        // Player 이동 
        void ManualMove()
        {
            if (_pv.IsMine)
            {
                if (!isReverseKey)
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
                    //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

                    Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                    transform.Translate(-moveDir.normalized * _velocity.magnitude * Time.deltaTime, Space.World);

                    if (_velocity != Vector3.zero)
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-moveDir), 0.35f);
                }
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

        //[PunRPC]
        //public void ChangeStateClientRpc(State newState)
        //{
        //    if (_pv.IsMine)
        //    {
        //        _animator.SetInteger("State", (int)newState);
        //        _animator.SetBool("IsDirty", true);
        //        Debug.Log("State : " + newState + "(" + (int)newState + ")" );
        //        Debug.Log("IsDirty : " + _animator.GetBool("IsDirty"));
        //    }
        //}

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(_animator.GetInteger("State"));
                stream.SendNext(_animator.GetBool("IsDirty"));
                Debug.Log("Send State : " + _animator.GetInteger("State"));
                Debug.Log("Send IsDirty : " + _animator.GetBool("IsDirty"));
            }
            else if(stream.IsReading)
            {
                transform.position = (Vector3)stream.ReceiveNext();
                transform.rotation = (Quaternion)stream.ReceiveNext();
                int state = (int)stream.ReceiveNext();
                bool isDirty = (bool)stream.ReceiveNext();
                Debug.Log("Receive State : " + state);
                Debug.Log("Receive IsDirty : " + isDirty);
                //_animator.SetBool("IsDirty", true)
                _animator.SetInteger("State", state);
                _animator.SetBool("IsDirty", isDirty);
            }
        }
    }
}
