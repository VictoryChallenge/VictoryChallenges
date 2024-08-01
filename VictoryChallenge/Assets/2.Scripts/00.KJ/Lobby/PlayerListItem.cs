using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace VictoryChallenge.KJ.Lobby
{
    public class PlayerListItem : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_Text text;             // �÷��̾� �̸� �ؽ�Ʈ

        Player player;                              // �÷��̾� ����

        public void SetUp(Player _player)
        {
            player = _player;
            text.text = _player.NickName;
            UpdateReadyState(); // �ܺο��� ȣ���Ͽ� ���� ����
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
                    text.color = Color.red; // �غ���� ���� �÷��̾�� ���������� ǥ��
                    Debug.Log($"{player.NickName} �غ� �ȵ�(������)");
                }
                else
                {
                    text.color = Color.blue;
                    Debug.Log($"{player.NickName} �غ��(������)");
                }
            }
            else
            {
                text.color = Color.magenta; // �غ� ���°� �������� ���� ��쵵 ���������� ǥ��
                Debug.LogWarning($"{player.NickName} �غ� ������Ƽ�� ����(������)");
            }
        }

        private void UpdateReadyState(bool isReady)
        {
            if (!(bool)isReady)
            {
                text.color = Color.red; // �غ���� ���� �÷��̾�� ���������� ǥ��
                Debug.Log($"{player.NickName} �غ� �ȵ�(������)");
            }
            else
            {
                text.color = Color.blue;
                Debug.Log($"{player.NickName} �غ��(������)");
            }
        }

        public void Refresh()
        {
            UpdateReadyState(); // �ܺο��� ȣ���Ͽ� ���� ����
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
