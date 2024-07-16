using Unity.VisualScripting;
using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class ConveyorBelt : MonoBehaviour
    {
        private float conveyorSpeed = 10f;
        private float obstacleSpeed = 5f;
        private int correction = 10;
        private float elapsedTime = 0f;
        public LayerMask obstacleLayer;

        void FixedUpdate()
        {
            // 모든 플레이어가 준비가 끝나고 321 후에 시간이 돌아가도록 설정해야할듯
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 20f)
            {
                conveyorSpeed = 20f;
                Debug.Log($"20초 경과 : {conveyorSpeed}");
            }
            else if (elapsedTime >= 10f)
            {
                conveyorSpeed = 15f;
                Debug.Log($"10초 경과 : {conveyorSpeed}");
            }
            else
            {
                conveyorSpeed = 10f;
                Debug.Log($"현재 컨베이어 스피드 : {conveyorSpeed}");
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            Rigidbody rb = collision.rigidbody;

            if (rb != null)
            {
                Vector3 forceDirection = -transform.forward;
                rb.AddForce(forceDirection * (conveyorSpeed * correction));
            }

            ObstacleManager obstacle = collision.gameObject.GetComponent<ObstacleManager>();
            if (obstacle != null && ((1 << collision.gameObject.layer) & obstacleLayer) == 0)
            {
                Vector3 forceDirection = -transform.forward;
                obstacle.SetConveyorSpeed(forceDirection, conveyorSpeed);
            }
        }
    }
}