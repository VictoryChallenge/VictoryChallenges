using Photon.Pun;
using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class ConveyorBelt : MonoBehaviourPun
    {
        private float _conveyorSpeed = 10f;
        private float _obstacleSpeed = 8f;
        private int _correction = 10;
        private float _elapsedTime = 0f;
        private bool _isConveyorEnabled = false;


        void Update()
        {
            if (_isConveyorEnabled == true)
            {
                _elapsedTime += Time.deltaTime;

                if (_elapsedTime > 30f)
                {
                    _conveyorSpeed = 30f;
                    _obstacleSpeed = 20f;
                }
                else if (_elapsedTime > 20f)
                {
                    _conveyorSpeed = 20f;
                    _obstacleSpeed = 10f;
                    Debug.Log($"20�� ��� : {_conveyorSpeed}");
                }
                else if (_elapsedTime > 10f)
                {
                    _conveyorSpeed = 15f;
                    _obstacleSpeed = 12f;
                    Debug.Log($"10�� ��� : {_conveyorSpeed}");
                }
                else
                {
                    _conveyorSpeed = 10f;
                    _obstacleSpeed = 8f;
                    Debug.Log($"���� �����̾� ���ǵ� : {_conveyorSpeed}");
                }
            }

            else
            {
                _conveyorSpeed = 0f;
                _obstacleSpeed = 0f;
                Debug.Log($"���� �����̾� ���ǵ� : {_conveyorSpeed}");
            }
        }

        public void EnableConveyerBelt()
        {
            _isConveyorEnabled = true;
            Debug.Log($"���� : �����̾Ʈ �۵�");
        }

        public void DisableConveyerBelt()
        {
            _isConveyorEnabled = false;
            Debug.Log($"���� : �����̾Ʈ ����");
        }

        public void Initialize()
        {
            _conveyorSpeed = 0f;
            _elapsedTime = 0f;
            _isConveyorEnabled = false;
        }


        private void OnCollisionStay(Collision collision)
        {
            Rigidbody rb = collision.rigidbody;
            Transform trans = collision.transform;
            Debug.Log(rb.name);

            if (rb != null)
            {
                Vector3 forceDirection = -transform.forward;
                Vector3 moveDirection = -transform.forward * Time.fixedDeltaTime;
                if (collision.gameObject.CompareTag("Obstacle"))
                {
                    trans.Translate(moveDirection * _obstacleSpeed, Space.World);
                    Debug.Log($"��ֹ� �ӵ� ����: {collision.gameObject.name} with speed {_obstacleSpeed * _correction}");
                }
                else
                {
                    rb.AddForce(forceDirection * (_conveyorSpeed * _correction));
                    Debug.Log($"�÷��̾� �ӵ� ����: {collision.gameObject.name} with speed {_conveyorSpeed * _correction}");
                }
            }
        }
    }
}