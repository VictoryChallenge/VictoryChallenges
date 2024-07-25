using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using QFSW.QC.Utilities;

public class ObstacleEffect : MonoBehaviour
{
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private Transform _hitPoint;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Instantiate(_hitEffect, _hitPoint);
            _hitEffect.SetActive(true);
        }
    }

    
}
