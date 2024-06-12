using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace VictoryChallenge.KJ.Lobby
{
    public class PlayerListItem : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_Text text;             // 플레이어 이름 텍스트

        Player player;                              // 플레이어 참조

        public void SetUp(Player _player)
        {
            player = _player;
            text.text = _player.NickName;
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (player == otherPlayer)
            {
                Destroy(gameObject);
            }
        }

        public override void OnLeftRoom()
        {
            Destroy(gameObject);
        }
    }
}
