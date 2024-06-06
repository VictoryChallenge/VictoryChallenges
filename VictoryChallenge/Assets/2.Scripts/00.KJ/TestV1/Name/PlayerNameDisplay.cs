using Photon.Pun;
using TMPro;
using UnityEngine;

namespace VictoryChallenge.KJ.Name
{
    public class PlayerNameDisplay : MonoBehaviour
    {
        [SerializeField] PhotonView playerPV;       // Player Photon View
        [SerializeField] TMP_Text text;             // �÷��̾� �̸�

        void Start()
        {
            if (playerPV.IsMine)
            {
                gameObject.SetActive(true);
            }

            text.text = playerPV.Owner.NickName;
        }
    }
}
