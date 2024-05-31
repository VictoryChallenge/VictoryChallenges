using Photon.Pun;
using Photon.Realtime;
using System.IO;
using TMPro;
using UnityEngine;
using VictoryChallenge.KJ.Photon;

namespace VictoryChallenge.KJ.Room
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] TMP_Text text;     // 텍스트

        RoomInfo info;                      // 방 정보 저장

        public void Setup(RoomInfo _info)
        {
            info = _info;
            text.text = _info.Name;
        }

        public void OnClick()
        {
            PhotonLauncher.Instance.JoinRoom(info);
        }
    }
}

