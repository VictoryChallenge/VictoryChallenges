using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        private float _offsetY = 2f;
        private Transform _playerTransform;


        void Start()
        {
            _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        }

        void Update()
        {
            Vector3 newPos = new Vector3(_playerTransform.position.x, _playerTransform.position.y + _offsetY, _playerTransform.position.z);
            transform.position = newPos;
        }
    }
}

