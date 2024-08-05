using UnityEngine;

public class RotateObstacle : MonoBehaviour
{
    private Vector3 _previousRotation;
    private Transform _playerTransform;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _playerTransform = collision.transform;
            _previousRotation = transform.rotation.eulerAngles;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // 플랫폼의 회전 변화량 계산
            Vector3 currentRotation = transform.rotation.eulerAngles;
            float rotationDelta = currentRotation.y - _previousRotation.y;

            // 회전 변화량만큼 플레이어를 중심을 기준으로 회전
            _playerTransform.RotateAround(transform.position, Vector3.up, rotationDelta);

            // 이전 회전 값 업데이트
            _previousRotation = currentRotation;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _playerTransform = null;
        }
    }
}
