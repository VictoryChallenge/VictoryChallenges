using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallRespawn : MonoBehaviour
{
    [SerializeField] private Vector3 _respawnPoint;
    [SerializeField] private GameObject _respawnEffect;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.transform.position = _respawnPoint;

            Instantiate(_respawnEffect, other.transform);
        }
    }
}
