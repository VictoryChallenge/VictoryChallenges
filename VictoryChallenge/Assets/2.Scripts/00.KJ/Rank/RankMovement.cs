using Photon.Pun;
using UnityEngine;

namespace VictoryChallenge.KJ.Rank
{
    public class RankMovement : MonoBehaviourPunCallbacks, IPunObservable
    {

        private Vector3 playerPosition;

        void Update()
        {
            if (photonView.IsMine)
            {
                // �÷��̾� ��ġ ������Ʈ
                playerPosition = transform.position;
            }
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // �ڽ��� ��ġ ����
                stream.SendNext(playerPosition);
            }
            else
            {
                // �ٸ� �÷��̾� ��ġ ����
                playerPosition = (Vector3)stream.ReceiveNext();
                transform.position = playerPosition;
            }
        }
    }
}
