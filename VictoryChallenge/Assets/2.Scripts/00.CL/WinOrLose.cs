using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.CL
{ 
    public class WinOrLose : MonoBehaviour
    {
        void Start()
        {
            PhotonNetwork.LeaveRoom();

            GameObject.Find("Exit").GetComponent<Button>().onClick.AddListener(Exit);
        }

        void Update()
        {

        }

        void Exit()
        {
            // 로비로 돌아가기
            SceneManager.LoadScene(1);
            Managers.Sound.Play("MainBGM", Define.Sound.BGM);
        }
    }
}
