using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float force;
    public float arcHeight;
    private TestMove testMove;
    public Vector3 des;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            testMove = other.GetComponent<TestMove>();

            if(rb != null && testMove != null)
            {
                testMove.enabled = false;

                
                float distance = Vector3.Distance(other.transform.position, des);

                Vector3 forceDir = transform.right + Vector3.up * Mathf.Sqrt(2 * arcHeight * Physics.gravity.magnitude / Mathf.Pow(distance / force, 2));
                rb.velocity = forceDir * force;
            }
        }
    }

    
}
