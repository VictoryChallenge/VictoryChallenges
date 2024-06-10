using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VictoryChallenge.KJ.Lobby
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        #region Lobby
        public override void OnLeftRoom()                   // �κ�(��)���� �������� ȣ��
        {
            base.OnLeftRoom();
            CleanUpPhotonView();
            PhotonNetwork.LoadLevel(0);                     // �޴� ������ �̵�
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        void CleanUpPhotonView()                            // Photon ������Ʈ ����
        {
            foreach (PhotonView pv in FindObjectsOfType<PhotonView>())
            {
                if (pv.IsMine && pv.ViewID > 0)
                {
                    Destroy(pv.gameObject);
                }
            }
        }
        #endregion
    }
}

