using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace VictoryChallenge.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _cameraKeyBoardRotateSpeed = 1.5f;
        [SerializeField] private float _cameraMouseRotateSpeed = 4.5f;
        [SerializeField] private float _cameraMouseUpSpeed = 3f;

        private float _offsetYMax = 4f;
        private float _offsetYMin = -0.5f;

        private CinemachineVirtualCamera _vCam;
        private CinemachineTrackedDolly _settingCam;
        

        void Start()
        {
            _vCam = GetComponent<CinemachineVirtualCamera>();
            _settingCam = _vCam.GetCinemachineComponent<CinemachineTrackedDolly>();

            transform.parent = null;
        }

        void Update()
        {
            // 카메라 시점이 오른쪽으로 이동
            if (Input.GetKey(KeyCode.Q))
            {
                _settingCam.m_PathPosition += _cameraKeyBoardRotateSpeed * Time.deltaTime;
            }

            // 카메라 시점이 왼쪽으로 이동
            if (Input.GetKey(KeyCode.E))
            {
                _settingCam.m_PathPosition -= _cameraKeyBoardRotateSpeed * Time.deltaTime;
            }

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
 
            if(mouseX > 0)
            {
                _settingCam.m_PathPosition += _cameraMouseRotateSpeed * Time.deltaTime;
            }
            else if(mouseX < 0)
            {
                _settingCam.m_PathPosition -= _cameraMouseRotateSpeed * Time.deltaTime;
            }

            if(mouseY > 0)
            {
                _settingCam.m_PathOffset.y += _cameraMouseUpSpeed * Time.deltaTime;
            }
            else if(mouseY < 0)
            {
                _settingCam.m_PathOffset.y -= _cameraMouseUpSpeed * Time.deltaTime;
            }

            _settingCam.m_PathOffset.y = Mathf.Clamp(_settingCam.m_PathOffset.y, _offsetYMin, _offsetYMax);
        }
    }
}
