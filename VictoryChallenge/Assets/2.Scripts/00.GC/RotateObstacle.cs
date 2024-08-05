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
                // �÷����� ȸ�� ��ȭ�� ���
                Vector3 currentRotation = transform.rotation.eulerAngles;
                float rotationDelta = currentRotation.y - _previousRotation.y;

                // ȸ�� ��ȭ����ŭ �÷��̾ �߽��� �������� ȸ��
                _playerTransform.RotateAround(transform.position, Vector3.up, rotationDelta);

                // ���� ȸ�� �� ������Ʈ
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
