using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace VictoryChallenge.Scripts.CL
{
    public class CreateRoom : MonoBehaviourPunCallbacks
    {

        public TMP_InputField roomnameInputfield;

        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void CreateRoom1()
        {
            string roomName = roomnameInputfield.text;
            if (string.IsNullOrEmpty(roomName))
            {

            }

            
        }
    }
}

