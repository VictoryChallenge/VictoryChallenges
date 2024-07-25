using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObstacleEffect : MonoBehaviour
{
    [SerializeField] private GameObject _hitEffect;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        Instantiate(_hitEffect, transform);
    }


}
