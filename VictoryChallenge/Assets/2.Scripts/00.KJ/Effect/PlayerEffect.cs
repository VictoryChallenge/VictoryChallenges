using UnityEngine;

namespace VictoryChallenge.KJ.Effect
{
    public class PlayerEffect : MonoBehaviour
    {
        public GameObject runEffectobj;
        public GameObject jumpEffectobj;
        public float jumpEffectDuration = 0.2f;

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
                ActiveJumpEffect();
            }
        }

        private void ActiveJumpEffect()
        {
            jumpEffectobj.SetActive(true);
            Debug.Log($"���� Ȱ��ȭ : {jumpEffectobj.activeSelf}");
            Invoke("DeactivateJumpEffect", jumpEffectDuration);
        }

        private void DeactivateJumpEffect()
        {
            jumpEffectobj.SetActive(false);
            Debug.Log($"���� ��Ȱ��ȭ : {jumpEffectobj.activeSelf}");
        }
    }
}
