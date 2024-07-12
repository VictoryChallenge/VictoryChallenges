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
                // 플레이어 위치 업데이트
                playerPosition = transform.position;
            }
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // 자신의 위치 전송
                stream.SendNext(playerPosition);
            }
            else
            {
                // 다른 플레이어 위치 수신
                playerPosition = (Vector3)stream.ReceiveNext();
                transform.position = playerPosition;
            }
        }
    }
}
