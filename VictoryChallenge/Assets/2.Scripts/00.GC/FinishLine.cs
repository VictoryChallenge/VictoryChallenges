using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    float _currentLap = 0;

    private void Start()
    {
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _currentLap++;

            if( _currentLap >= 3 )
            {
                _currentLap = 3;
            }
        }
    }
}
