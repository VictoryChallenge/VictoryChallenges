using Photon.Pun;
using UnityEngine;
using VictoryChallenge.ComponentExtensions;

namespace VictoryChallenge.KJ.Effect
{
    public class PlayerEffect : MonoBehaviourPun
    {
        public GameObject runEffectobj;
        public GameObject jumpEffectobj;
        public GameObject dizzyEffectobj;
        public float jumpEffectDuration = 0.2f;

        void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }

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
            if (Input.GetKeyDown(KeyCode.Space) && this.IsGrounded())
            {
                ActivateJumpEffect();
            }
        }

        private void ActivateJumpEffect()
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

        public void ActivateDizzyEffect()
        {
            if (dizzyEffectobj != null)
            {
                dizzyEffectobj.SetActive(true);
                Debug.Log("����!!!!!!!!!!!!!!!!!!!");
            }
        }

        public void DeactivateDizzyEffect()
        {
            if (dizzyEffectobj != null)
            {
                dizzyEffectobj.SetActive(false);
                Debug.Log("���� ����");
            }
        }
    }
}
