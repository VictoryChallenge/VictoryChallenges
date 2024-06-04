using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Scripts.HS
{
    public class CollisionTest : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
            {
                Debug.Log("collision detect footCollider");
            }
        }
    }

}
