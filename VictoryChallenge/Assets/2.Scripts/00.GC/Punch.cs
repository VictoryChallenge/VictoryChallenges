using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Punch : MonoBehaviour
{
    private float _punchPower = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            if(rb != null )
            {
                rb.AddForce(Vector3.right * _punchPower, ForceMode.Impulse);
            }    
        }
    }
}
