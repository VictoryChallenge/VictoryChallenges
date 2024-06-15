using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VictoryChallenge.Map
{
    public class Gravity : MonoBehaviour
    {

        private void OnTriggerStay(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                other.GetComponent<Rigidbody>().drag = 10f;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                other.GetComponent<Rigidbody>().drag = 0f;
            }
        }
    }
}