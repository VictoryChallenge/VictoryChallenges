using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class Boost : MonoBehaviour
    {
        private float _boostSpeed = 35f;
        private float _correction = 10f;

        private void OnCollisionStay(Collision collision)
        {
            Rigidbody rb = collision.rigidbody;

            if (rb != null)
            {
                Vector3 forceDirection = -transform.forward;
                rb.AddForce(forceDirection * (_boostSpeed * _correction));
            }
        }
    }

}