using Photon.Pun;
using UnityEngine;

public class RotateObstacle : MonoBehaviour
{
    private Vector3 _previousRotation;
    private Transform _playerTransform;
    private PhotonView _pv;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PhotonView playerPhotonView = collision.gameObject.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            { 
                _playerTransform = collision.transform;
                _previousRotation = transform.rotation.eulerAngles;
                Debug.LogError("1");
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PhotonView playerPhotonView = collision.gameObject.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            { 
                // 플랫폼의 회전 변화량 계산
                Vector3 currentRotation = transform.rotation.eulerAngles;
                float rotationDelta = currentRotation.y - _previousRotation.y;

                // 회전 변화량만큼 플레이어를 중심을 기준으로 회전
                _playerTransform.RotateAround(transform.position, Vector3.up, rotationDelta);

                // 이전 회전 값 업데이트
                _previousRotation = currentRotation;
                Debug.LogError("2");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PhotonView playerPhotonView = collision.gameObject.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            { 
                _playerTransform = null;
                Debug.LogError("3");
            }
        }
    }
}
