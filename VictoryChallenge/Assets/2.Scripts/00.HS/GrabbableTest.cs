using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.Controllers.Player;

public class GrabbableTest : MonoBehaviour
{
    private PlayerController _characterController;


    void Start()
    {
        _characterController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            _characterController.isGrabbable = true;

            _characterController.grabbableTransform = other.gameObject.GetComponent<Transform>();
            _characterController.grabbableRigid = other.gameObject.GetComponent<Rigidbody>();
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            _characterController.isGrabbable = false;
        }
    }
}
