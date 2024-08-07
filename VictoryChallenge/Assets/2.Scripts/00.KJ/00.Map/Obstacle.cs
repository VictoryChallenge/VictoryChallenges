using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class Obstacle : MonoBehaviour
    {
        public float minYPoisition = -5f;
        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (transform.position.y < minYPoisition)
            {
                Destroy(gameObject);
            }
        }

        void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                rb.constraints = RigidbodyConstraints.FreezePositionY;
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }
}
