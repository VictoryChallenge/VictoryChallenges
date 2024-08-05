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
        if (_pv.IsMine)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _playerTransform = collision.transform;
                _previousRotation = transform.rotation.eulerAngles;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (_pv.IsMine)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // �÷����� ȸ�� ��ȭ�� ���
                Vector3 currentRotation = transform.rotation.eulerAngles;
                float rotationDelta = currentRotation.y - _previousRotation.y;

                // ȸ�� ��ȭ����ŭ �÷��̾ �߽��� �������� ȸ��
                _playerTransform.RotateAround(transform.position, Vector3.up, rotationDelta);

                // ���� ȸ�� �� ������Ʈ
                _previousRotation = currentRotation;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_pv.IsMine)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _playerTransform = null;
            }
        }
    }
}
