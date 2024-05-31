using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterfall : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Àß°¡¶ó ¤»¤»");
    }
}
