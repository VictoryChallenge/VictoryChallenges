using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VictoryChallenge.Map
{
    public class Goal : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log("¿Í Â¦Â¦Â¦!!");
            }
        }
    }
}