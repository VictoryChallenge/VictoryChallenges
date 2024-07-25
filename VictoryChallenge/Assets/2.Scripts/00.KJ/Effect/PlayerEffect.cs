using UnityEngine;

namespace VictoryChallenge.KJ.Effect
{
    public class PlayerEffect : MonoBehaviour
    {
        public GameObject runEffectobj;
        public GameObject jumpEffectobj;

        void Update()
        {
            // 달리기 이펙트
            if (Input.GetKey(KeyCode.LeftShift))
            {
                runEffectobj.SetActive(true);
            }
            else
            {
                runEffectobj.SetActive(false);
            }

            // 점프 이펙트
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpEffectobj.SetActive(true);
            }
        }
    }
}
