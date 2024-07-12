using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VictoryChallenge.KJ.Photon;

namespace VictoryChallenge.KJ.Name
{
    public class PlayerNameDisplay : MonoBehaviour
    {
        [SerializeField] PhotonView playerPV;       // Player Photon View
        [SerializeField] TMP_Text text;             // �÷��̾� �̸�
        [SerializeField] Image readyImage;          // ���� �̹���

        

        void Start()
        {
            if (playerPV.IsMine)
            {
                gameObject.SetActive(true);
            }

            text.text = playerPV.Owner.NickName;
        }

        /// <summary>
        /// ��ư�� �Է½� �̹��� ��� ���� 
        /// </summary>
        public void ToggleReadyState()
        {
            PhotonSub.Instance._isReady = !PhotonSub.Instance._isReady;
            UpdateReadyImage();
        }

        void UpdateReadyImage()
        {
            readyImage.enabled = PhotonSub.Instance._isReady;
        }
    }
}
