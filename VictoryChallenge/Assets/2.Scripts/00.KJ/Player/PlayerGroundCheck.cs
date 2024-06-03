using UnityEngine;

namespace VictoryChallenge.KJ.Manager
{
    public class PlayerGroundCheck : MonoBehaviour
    {
        PlayerController playerController;

        void Awake()
        {
            playerController = GetComponent<PlayerController>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == playerController.gameObject)
                return;

            playerController.SetGroundedState(true);
        }
        void OnTriggerExit(Collider other)
        {
            if (other.gameObject == playerController.gameObject)
                return;

            playerController.SetGroundedState(false);
        }

        void OnTriggerStay(Collider other)
        {
            if (other.gameObject == playerController.gameObject)
                return;

            playerController.SetGroundedState(true);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == playerController.gameObject)
                return;

            playerController.SetGroundedState(true);
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject == playerController.gameObject)
                return;

            playerController.SetGroundedState(false);
        }

        void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject == playerController.gameObject)
                return;

            playerController.SetGroundedState(true);
        }
    }
}
