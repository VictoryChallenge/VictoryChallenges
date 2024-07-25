using UnityEngine;

namespace VictoryChallenge.KJ.Effect
{
    public class PlayerEffect : MonoBehaviour
    {
        public GameObject runEffectobj;
        public GameObject jumpEffectobj;

        void Update()
        {
            // �޸��� ����Ʈ
            if (Input.GetKey(KeyCode.LeftShift))
            {
                runEffectobj.SetActive(true);
            }
            else
            {
                runEffectobj.SetActive(false);
            }

            // ���� ����Ʈ
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpEffectobj.SetActive(true);
            }
        }
    }
}
