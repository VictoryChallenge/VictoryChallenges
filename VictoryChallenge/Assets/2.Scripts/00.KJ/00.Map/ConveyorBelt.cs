using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class ConveyorBelt : MonoBehaviour
    {
        private float conveyorSpeed = 10f;
        private float obstacleSpeed = 8f;
        private int correction = 10;
        private float elapsedTime = 0f;

        void FixedUpdate()
        {
            // ��� �÷��̾ �غ� ������ 321 �Ŀ� �ð��� ���ư����� �����ؾ��ҵ�
            elapsedTime += Time.deltaTime;

            if (elapsedTime > 20f)
            {
                conveyorSpeed = 20f;
                Debug.Log($"20�� ��� : {conveyorSpeed}");
            }
            else if (elapsedTime > 10f)
            {
                conveyorSpeed = 15f;
                Debug.Log($"10�� ��� : {conveyorSpeed}");
            }
            else
            {
                conveyorSpeed = 10f;
                Debug.Log($"���� �����̾� ���ǵ� : {conveyorSpeed}");
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            Rigidbody rb = collision.rigidbody;
            Transform trans = collision.transform;
            Debug.Log(rb.name);

            if (rb != null)
            {
                Vector3 forceDirection = -transform.forward;
                Vector3 moveDirection = -transform.forward * Time.fixedDeltaTime;
                if (collision.gameObject.CompareTag("Obstacle"))
                {
                    trans.Translate(moveDirection * obstacleSpeed, Space.World);
                    Debug.Log($"��ֹ� �ӵ� ����: {collision.gameObject.name} with speed {obstacleSpeed * correction}");
                }
                else
                {
                    rb.AddForce(forceDirection * (conveyorSpeed * correction));
                    Debug.Log($"�÷��̾� �ӵ� ����: {collision.gameObject.name} with speed {conveyorSpeed * correction}");
                }
            }
        }
    }
}