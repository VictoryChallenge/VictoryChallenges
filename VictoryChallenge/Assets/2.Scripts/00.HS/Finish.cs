using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;

namespace VictoryChallenge.Scripts.HS
{
    public class Finish : MonoBehaviour
    {
        private int _rankCount;

        void Start()
        {
        
        }

        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if(!other.gameObject.GetComponent<CharacterController>().isFinished)
                {
                    _rankCount++;
                    other.gameObject.GetComponent<CharacterController>().isFinished = true;
                }
                Debug.Log("들어온 플레이어 수 : " + _rankCount);
            }
        }
    }
}
