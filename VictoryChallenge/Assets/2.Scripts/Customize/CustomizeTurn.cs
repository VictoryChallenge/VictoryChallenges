using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Customize
{
    public class CustomizeTurn : MonoBehaviour
    {
        [SerializeField] private float _tureSpeed = 10f;
        private Transform _playerTransform;


        void Start()
        {
            _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        }

        void Update()
        {
            if(Input.GetKey(KeyCode.Q))
            {
                _playerTransform.Rotate(new Vector3(0, 20f, 0) * _tureSpeed * Time.deltaTime);
                //_playerTransform.rotation = Quaternion.Euler(0f, _playerTransform.rotation.y - _tureSpeed * Time.deltaTime, 0f);
            }

            if(Input.GetKey(KeyCode.E))
            {
                _playerTransform.Rotate(new Vector3(0, -20f, 0) * _tureSpeed * Time.deltaTime);
                //_playerTransform.rotation = Quaternion.Euler(0f, _playerTransform.rotation.y + _tureSpeed * Time.deltaTime, 0f);
            }
        }
    }

}
