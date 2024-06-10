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
        public override void OnLeftRoom()                   // 로비(룸)에서 떠났으면 호출
        {
            base.OnLeftRoom();
            CleanUpPhotonView();
            PhotonNetwork.LoadLevel(0);                     // 메뉴 씬으로 이동
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        void CleanUpPhotonView()                            // Photon 오브젝트 삭제
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

