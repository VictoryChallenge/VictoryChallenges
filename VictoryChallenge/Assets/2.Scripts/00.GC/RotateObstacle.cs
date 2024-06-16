using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VictoryChallenge.Map
{
    public class RotateObstacle : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                collision.transform.SetParent(transform);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                collision.transform.SetParent(null);
            }
        }
    }
}