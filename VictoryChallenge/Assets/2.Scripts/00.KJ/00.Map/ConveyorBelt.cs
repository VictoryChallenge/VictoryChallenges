using Photon.Pun;
using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class ConveyorBelt : MonoBehaviourPun
    {
        private float conveyorSpeed = 10f;
        private float obstacleSpeed = 8f;
        private int correction = 10;
        private float elapsedTime = 0f;
        private bool isConveyorEnabled = false;


        void Update()
        {
            if (isConveyorEnabled)
            {
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
        }

        public void EnableConveyerBelt()
        {
            isConveyorEnabled = true;
            Debug.Log($"���� : �����̾Ʈ �۵�");
        }

        public void Initialize()
        {
            conveyorSpeed = 0f;
            elapsedTime = 0f;
            isConveyorEnabled = false;
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