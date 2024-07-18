using GSpawn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pool : MonoBehaviourPun
{
    [PunRPC]
    void SetActiveRPC(bool b)
    {
        gameObject.SetActive(b);
        Debug.Log("¿€µø");
    }
}
