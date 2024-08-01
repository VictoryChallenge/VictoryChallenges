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
            UpdateReadyState(); // 외부에서 호출하여 상태 갱신
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (player == otherPlayer)
            {
                Destroy(gameObject);
            }
        }

        private void UpdateReadyState()
        {
            object isReady;

            if (player.CustomProperties.TryGetValue("IsReady", out isReady))
            {
                if (!(bool)isReady)
                {
                    text.color = Color.red; // 준비되지 않은 플레이어는 빨간색으로 표시
                    Debug.Log($"{player.NickName} 준비 안됨(아이템)");
                }
                else
                {
                    text.color = Color.blue;
                    Debug.Log($"{player.NickName} 준비됨(아이템)");
                }
            }
            else
            {
                text.color = Color.magenta; // 준비 상태가 설정되지 않은 경우도 빨간색으로 표시
                Debug.LogWarning($"{player.NickName} 준비 프로퍼티가 없음(아이템)");
            }
        }

        private void UpdateReadyState(bool isReady)
        {
            if (!(bool)isReady)
            {
                text.color = Color.red; // 준비되지 않은 플레이어는 빨간색으로 표시
                Debug.Log($"{player.NickName} 준비 안됨(아이템)");
            }
            else
            {
                text.color = Color.blue;
                Debug.Log($"{player.NickName} 준비됨(아이템)");
            }
        }

        public void Refresh()
        {
            UpdateReadyState(); // 외부에서 호출하여 상태 갱신
        }

        public void Refreshbool(bool isReady)
        { 
            UpdateReadyState(isReady);
        }

        public override void OnLeftRoom()
        {
            Destroy(gameObject);
        }
    }
}
