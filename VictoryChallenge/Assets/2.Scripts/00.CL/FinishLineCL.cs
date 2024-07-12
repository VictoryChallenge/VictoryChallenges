using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.KJ.Photon;

namespace VictoryChallenge.Scripts.CL
{
    public class FinishLineCL : MonoBehaviour
    {
        float finishTime = 0f;

        private void Start()
        {
            finishTime += Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                PhotonSub.Instance.OnPlayerFinish(PhotonNetwork.LocalPlayer.UserId, finishTime);
            }
        }
    }
}