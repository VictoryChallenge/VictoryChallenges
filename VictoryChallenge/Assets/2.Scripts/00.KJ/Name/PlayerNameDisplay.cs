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
        [SerializeField] TMP_Text text;             // 플레이어 이름
        [SerializeField] Image readyImage;          // 레디 이미지

        

        void Start()
        {
            if (playerPV.IsMine)
            {
                gameObject.SetActive(true);
            }

            text.text = playerPV.Owner.NickName;
        }

        /// <summary>
        /// 버튼에 입력시 이미지 토글 형식 
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
