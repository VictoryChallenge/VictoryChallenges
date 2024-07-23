using VictoryChallenge.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using VictoryChallenge.ComponentExtensions;
using Cinemachine;
using VictoryChallenge.Customize;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System;
using UnityEngine.UI;
using VictoryChallenge.Scripts.CL;
using VictoryChallenge.KJ.Effect;

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
        public PhotonView _pv;
        #endregion

        #region 코루틴
        public Coroutine comboStackResetCoroutine;
        #endregion

        #region 상호 작용
        // Grab
        public bool isGrabbing
        {
            get => _isGrabbing;
            set => _isGrabbing = value;
        }

        private bool _isGrabbing;
        public virtual bool isGrabbable { get; set; }
        public virtual Transform grabbableTransform { get; set; }
        public virtual Rigidbody grabbableRigid { get; set; }
        public virtual CapsuleCollider grabbableCollider { get; set; }
        public virtual GameObject holdingObject { get; set; }

        // Holding
        public bool isHolding
        {
            get => _isHolding;
            set => _isHolding = value;
        }

        private bool _isHolding;

        // Hit
        public virtual bool isDizzy { get; set; }
        public virtual bool isDie { get; set; }

        public virtual int hitCount { get; set; }
        public virtual int dizzyCount { get; set; }

        // Attack
        public bool isAttacking
        {
            get => _isAttacking;
            set => _isAttacking = value;
        }

        private bool _isAttacking;

        public bool isHit
        {
            get => _isHit;
            set => _isHit = value;
        }

        private bool _isHit;

        // Object
        public virtual bool isReverseKey { get; set; }

        public bool isSlip
        {
            get => _isSlip;
            set => _isSlip = value;
        }

        private bool _isSlip;

        public bool isSliping
        {
            get => _isSliping;
            set => _isSliping = value;
        }
        private bool _isSliping;

        // GoalIn Finish
        public bool isFinished
        {
            get => _isFinished;
            set => _isFinished = value;
        }

        private bool _isFinished;
        #endregion

        #region 카메라
        private CinemachineVirtualCamera _followCam;
        private CinemachineVirtualCamera _introCam;
        public PlayableDirector introTimeline { get => _introTimeline; }
        private PlayableDirector _introTimeline;

        private Image _missionUI;
        private GameManagerCL GameCL;
        #endregion

        #region DB
        public virtual string shortUID { get; set; }
        public virtual string nickName { get; private set; }
        #endregion


        private void Awake()
        {
            // 컴포넌트 캐싱
            _rigidBody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();

            // 애니메이션 상태머신 등록
            InitAnimatorBehaviours();

            // 포톤뷰 캐싱
            _pv = GetComponent<PhotonView>();
        }

        protected virtual void Start()
        {
            _followCam = transform.Find("VCam_Perspective").GetComponent<CinemachineVirtualCamera>();

            if (!_pv.IsMine)
            {
                //CinemachineVirtualCamera otherCam = transform.Find("VCam_Perspective").GetComponent<CinemachineVirtualCamera>();
                //otherCam.enabled = false;
                //_followCam = transform.Find("VCam_Perspective").GetComponent<CinemachineVirtualCamera>();
                _followCam.enabled = false;
                return;
            }

            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2))
            {
                nickName = PhotonNetwork.NickName;
            }
            else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(3))
            {
                nickName = PhotonNetwork.NickName;


                // 카메라 캐싱
                _introCam = GameObject.Find("IntroCam").GetComponent<CinemachineVirtualCamera>();
                _introTimeline = GameObject.Find("IntroTimeline").GetComponent<PlayableDirector>();
                _followCam.enabled = false;
                _introTimeline.stopped += OnStopTimeline;

                //GameCL = GameObject.Find("GameCL").GetComponent<GameManagerCL>();
                //_missionUI = GameObject.Find("Mission").GetComponent<Image>();
                //_missionUI.enabled = false;
            }
            else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(6))
            {
                nickName = PhotonNetwork.NickName;

                _introCam = GameObject.Find("IntroCam").GetComponent<CinemachineVirtualCamera>();
                _introTimeline = GameObject.Find("IntroTimeline").GetComponent <PlayableDirector>();
                _followCam.enabled = false;
                _introTimeline.stopped += OnStopTimeline;
            }
        }

        private void OnStopTimeline(PlayableDirector director)
        {
            _followCam.enabled = true;
            _introCam.enabled = false;
            //_missionUI.enabled = false;
        }

        private void FixedUpdate()
        {
            ManualMove();
        }

        protected virtual void Update()
        {
            if (!_pv.IsMine)
            {
                //if (transform.IsGrounded())
                //{
                //    Debug.Log("Client Ground Collision");
                //}
                return;
            }

            // 키 입력에 따른 이동값 설정
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if(_isKeyActive)
            {
                _velocity = new Vector3(horizontal, 0f, vertical).normalized * speedGain;
            }
            else
            {
                _velocity = Vector3.zero;
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

        // Player 이동 
        void ManualMove()
        {
            if (!_pv.IsMine)
            {
                return;
            }

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

        public IEnumerator C_IntroCutSceneStart()
        {
            _introTimeline.Play();
            Debug.Log("IntroTimeline play");

            while (_introTimeline.state == PlayState.Playing)
            {
                //if (_introTimeline.time > 5f)
                //{
                //    _missionUI.enabled = true;
                //}
                yield return null;
            }
        }

        //[PunRPC]
        //public void ChangeStateRPC(State newState)
        //{
        //    _animator.SetInteger("State", (int)newState);
        //    _animator.SetBool("IsDirty", true);
        //}

        [PunRPC]
        public void HitCheckRPC(bool isCheck)
        {
            _isHit = isCheck;
            Debug.Log("hitCheck" + _isHit);
        }

        [PunRPC]
        public void HoldingCheckRPC(bool isCheck)
        {
            _isHolding = isCheck;
        }

        [PunRPC]
        public void CustomDataRPC()
        {
            GetComponentInChildren<PlayerCharacterCustomized>().Load();
        }

        #region 충돌체크
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                if(_isSlip == false)
                {
                    _isSlip = true;
                }
            }
        }
        #endregion
    }
}
