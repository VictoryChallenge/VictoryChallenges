using UnityEngine;

namespace VictoryChallenge.KJ.Map
{
    public class Obstacle : MonoBehaviour
    {
        public float minYPoisition = -5f;

        void Update()
        {
            if (transform.position.y < minYPoisition)
            {
                Destroy(gameObject);
            }
        }
    }
}
