using VictoryChallenge.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using System.IO;
using VictoryChallenge.ComponentExtensions;

namespace VictoryChallenge.Controllers.Player
{
    /// <summary>
    /// Player를 제어하기 위한 스크립트
    /// </summary>
    /// 
    public class CharacterController : MonoBehaviour/*PunCallbacks*/ /*, IPunObservable*/
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
        private bool _receiveIsDirty;
        private int _receiveState;

        private float _localNormalizeTime;
        private float _remoteNormalizeTime;
        [SerializeField] private float _lerpTime = 0.1f;

        private bool _isGround;
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
            
            if (!_pv.IsMine)
            {
                return;
            }
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
                
                //_receiveState = _animator.GetInteger("State");
                //_receiveState = _animator.GetCurrentAnimatorStateInfo(0).GetHashCode();
                //_receiveIsDirty = _animator.GetBool("IsDirty");
                //_isGround = transform.IsGrounded();
                //Debug.Log("State?" + _animator.GetCurrentAnimatorStateInfo(0).fullPathHash);

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
            //else
            //{
                //_localNormalizeTime = Mathf.Lerp(_localNormalizeTime, _remoteNormalizeTime, Time.deltaTime / _lerpTime);
                //_animator.Play(_receiveState, 0, _localNormalizeTime);
            //}
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

        [PunRPC]
        public void ChangeStateClientRpc(State newState)
        {
            if (_pv.IsMine)
            {
                _animator.SetInteger("State", (int)newState);
                _animator.SetBool("IsDirty", true);
                //Debug.Log("State : " + newState + "(" + (int)newState + ")");
                //Debug.Log("IsDirty : " + _animator.GetBool("IsDirty"));
            }
        }

        //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        //{
        //    if(stream.IsWriting)
        //    {
        //        //stream.SendNext(transform.position);
        //        //stream.SendNext(transform.rotation);
        //        stream.SendNext(_receiveState);
        //        stream.SendNext(_receiveIsDirty);
        //        //stream.
        //        //stream.SendNext(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        //        //Debug.Log("Send State : " + _receiveState);
        //        //Debug.Log("Send IsDirty : " + _receiveIsDirty);
        //    }
        //    else
        //    {
        //        //transform.position = (Vector3)stream.ReceiveNext();
        //        //transform.rotation = (Quaternion)stream.ReceiveNext();
        //        _receiveState = (int)stream.ReceiveNext();
        //        _receiveIsDirty = (bool)stream.ReceiveNext();
        //        //_remoteNormalizeTime = (float)stream.ReceiveNext();
        //        //Debug.Log("Receive State : " + _receiveState);
        //        //Debug.Log("Receive IsDirty : " + _receiveIsDirty);

        //        //PhotonAnimatorView anim = GetComponent<PhotonAnimatorView>();
        //        //anim.SetLayerSynchronized(0, PhotonAnimatorView.SynchronizeType.Continuous);

        //        //float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
        //        //Debug.Log("지연시간 : " + lag);

        //        //_animator.SetInteger("State", _receiveState);
        //        //_animator.SetBool("IsDirty", _receiveIsDirty);
        //    }
        //}
    }
}
