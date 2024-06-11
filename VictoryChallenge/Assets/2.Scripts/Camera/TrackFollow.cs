using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.Controllers.Player;

namespace VictoryChallenge.Camera
{
    public class TrackFollow : MonoBehaviour
    {
        [SerializeField] private float _offsetY = 2f;
        private Transform _playerTransform;


        void Start()
        {
            _playerTransform = FindObjectOfType<PlayerController>().GetComponent<Transform>();
                //GameObject.Find("Player").GetComponent<Transform>();
        }

        void Update()
        {
            if( _playerTransform != null )
            {
                Vector3 newPos = new Vector3(_playerTransform.position.x, _playerTransform.position.y + _offsetY, _playerTransform.position.z);
                transform.position = newPos;
            }
        }
    }
}

