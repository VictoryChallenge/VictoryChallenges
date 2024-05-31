using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VictoryChallenge.Controllers.Player;
using DG.Tweening;

public class Trampoline : MonoBehaviour
{
    private PlayerController _pc;
    public Vector3 des;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _pc = other.GetComponent<PlayerController>();

            if(_pc != null)
            {
                other.transform.DOJump(des, 10, 1, 5.0f);
            }
        }
    }

    
}
