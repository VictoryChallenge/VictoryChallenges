using Photon.Pun;
using ExitGames.Client.Photon;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VictoryChallenge.KJ.Photon;
using Photon.Realtime;

namespace VictoryChallenge.Scripts.CL
{
    public class GameManagerCL : MonoBehaviourPunCallbacks
    {
        // �� ��ũ��Ʈ�� ��� �÷��̾ ���� �ε��ߴ��� Ȯ���ϰ�, �� �� ������ �����ϴ� ������ �մϴ�.

        private void Start()
        {
            OnSceneLoaded();
        }

        void OnSceneLoaded()
        {
            if (SceneManager.GetActiveScene().buildIndex >= 3 && SceneManager.GetActiveScene().buildIndex != 4)
            {
                Debug.Log("����������[��");
                // �÷��̾��� �� �ε� ���¸� CustomProperties�� ����
                Hashtable props = new Hashtable
                {
                    { "SceneLoaded", true }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            // ����� CustomProperties Ȯ��
            if (changedProps.ContainsKey("SceneLoaded"))
            {
                // SceneLoaded�� ����Ǿ��� ���� RPC ȣ��
                photonView.RPC("PlayerLoadedScene", RpcTarget.All);
            }
        }       

        // ������ Ŭ���̾�Ʈ�� �� �÷��̾��� ���¸� Ȯ���ϰ�, ��� �÷��̾ ���� �ε������� ������ �����մϴ�.
        [PunRPC]
        void PlayerLoadedScene()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("zzz������Ŭ");
                // ��� �÷��̾ ���� �ε��ߴ��� Ȯ��
                if (PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("SceneLoaded") && (bool)player.CustomProperties["SceneLoaded"]))
                {
                    Debug.Log("�־ȳ����µ��ֿֿֿ֤Ǿ֤Ǿ֤Ǿ�");
                    // ��� �÷��̾ ���� �ε������� �˸��� RPC ȣ��
                    photonView.RPC("StartGame", RpcTarget.All);
                }
            }
        }

        [PunRPC]
        void StartGame()
        {
            // ���� ���� ������ ����
            GameSceneUI gameSceneUI = FindObjectOfType<GameSceneUI>();
            if (gameSceneUI != null)
            {
                gameSceneUI.StartCoroutine("GameStart");
            }
        }
    }
}
