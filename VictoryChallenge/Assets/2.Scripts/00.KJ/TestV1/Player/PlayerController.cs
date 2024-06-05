using Photon.Pun;
using UnityEngine;

namespace VictoryChallenge.KJ.Manager
{
    public class PlayerController : MonoBehaviourPunCallbacks
    {
        [SerializeField] float sprintSpeed;
        [SerializeField] float walkSpeed;
        [SerializeField] float jumpForce;
        [SerializeField] float smoothTime;

        bool isGrounded;
        Vector3 smoothMoveVelocity;
        Vector3 moveAmount;

        Rigidbody rb;
        PhotonView pv;
        PlayerManager playerManager;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            pv = GetComponent<PhotonView>();

            playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
        }

        void Update()
        {
            if (!pv.IsMine)
                return;

            Move();
            Jump();

            if (transform.position.y < -10)                         // 일정 이하 (10)로 떨어질 시에 죽음으로 처리
            {
                Die();
            }
        }

        void Move()
        {
            Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
        }

        void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.AddForce(transform.up * jumpForce);
            }
        }

        public void SetGroundedState(bool _isGrounded)
        {
            isGrounded = _isGrounded;
        }

        void FixedUpdate()
        {
            if (!pv.IsMine)
                return;

            rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.deltaTime);
        }

        void Die()
        {
            playerManager.Die();
        }
    }
}
