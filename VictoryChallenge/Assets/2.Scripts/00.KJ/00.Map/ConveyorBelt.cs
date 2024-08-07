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
                }
                else if (_elapsedTime > 10f)
                {
                    _conveyorSpeed = 15f;
                    _obstacleSpeed = 12f;
                }
                else
                {
                    _conveyorSpeed = 10f;
                    _obstacleSpeed = 8f;
                }
            }

            else
            {
                _conveyorSpeed = 0f;
                _obstacleSpeed = 0f;
            }
        }

        public void EnableConveyerBelt()
        {
            _isConveyorEnabled = true;
        }

        public void DisableConveyerBelt()
        {
            _isConveyorEnabled = false;
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

            if (rb != null)
            {
                Vector3 forceDirection = -transform.forward;
                Vector3 moveDirection = -transform.forward * Time.fixedDeltaTime;
                if (collision.gameObject.CompareTag("Obstacle"))
                {
                    trans.Translate(moveDirection * _obstacleSpeed, Space.World);
                }
                else
                {
                    rb.AddForce(forceDirection * (_conveyorSpeed * _correction));
                }
            }
        }
    }
}