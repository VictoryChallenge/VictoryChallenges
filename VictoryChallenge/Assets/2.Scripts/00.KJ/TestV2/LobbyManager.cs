using Photon.Pun;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VictoryChallenge.KJ.Lobby
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        public override void OnJoinedRoom()
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                Debug.Log("플레이어 매니저 생성");
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            }
        }
    }
}

