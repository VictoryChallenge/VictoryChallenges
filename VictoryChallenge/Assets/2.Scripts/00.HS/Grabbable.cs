using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VictoryChallenge.Controllers.Player;

public class Grabbable : MonoBehaviour
{
    private PlayerController _characterController;


    void Start()
    {
        _characterController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _characterController.isGrabbable = true;

            _characterController.grabbableTransform = other.gameObject.GetComponent<Transform>();
            _characterController.grabbableRigid = other.gameObject.GetComponent<Rigidbody>();
            _characterController.grabbableCollider = other.gameObject.GetComponent<CapsuleCollider>();
            _characterController.holdingObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _characterController.isGrabbable = false;
        }
    }
}
