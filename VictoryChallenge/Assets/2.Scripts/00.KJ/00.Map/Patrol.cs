using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class Patrol : MonoBehaviour
    {
        public Transform pointA;
        public Transform pointB;
        public float patrolSpeed = 2.0f;

        private Vector3 _targetPoint;

        void Start()
        {
            _targetPoint = pointB.position;
        }

        void Update()
        {
            PatrolMovement();
        }

        private void PatrolMovement()
        {
            if (Vector3.Distance(transform.position, _targetPoint) < 0.1f)
            {
                if (_targetPoint == pointA.position)
                {
                    _targetPoint = pointB.position;
                }
                else
                {
                    _targetPoint = pointA.position;
                }
                UpdateRotation();
            }

            transform.position = Vector3.MoveTowards(transform.position, _targetPoint, patrolSpeed * Time.deltaTime);
        }

        private void UpdateRotation()
        {
            if (_targetPoint == pointB.position)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else if (_targetPoint == pointA.position)
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
            }
        }
    }
}
