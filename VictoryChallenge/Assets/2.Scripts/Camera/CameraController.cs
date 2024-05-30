using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace VictoryChallenge.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _cameraSpeed = 1.5f;

        private CinemachineVirtualCamera _vCam;
        private CinemachineTrackedDolly _settingCam;
        

        void Start()
        {
            _vCam = GetComponent<CinemachineVirtualCamera>();
            _settingCam = _vCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        }

        void Update()
        {
            // ī�޶� ������ ���������� �̵�
            if(Input.GetKey(KeyCode.Q))
            {
                _settingCam.m_PathPosition += _cameraSpeed * Time.deltaTime;
            }

            // ī�޶� ������ �������� �̵�
            if (Input.GetKey(KeyCode.E))
            {
                _settingCam.m_PathPosition -= _cameraSpeed * Time.deltaTime;
            }
        }
    }
}
