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

            Managers.Sound.Clear();

            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 4:
                    Managers.Sound.Play("ChildCheer");
                    Managers.Sound.Play("Applause");
                    Invoke("Win", 1f);
                    break;
                case 5:
                    Invoke("Lose", 0.5f);
                    break;
            }
        }

        void Update()
        {

        }

        void Win()
        {
            Managers.Sound.Play("Applause2");
            Invoke("Win2", 4f);
        }
        void Win2()
        {
            Managers.Sound.Play("Applause");
        }

        void Lose()
        {
            Managers.Sound.Play("Lose");
            Managers.Sound.Play("Booing");
        }

        void Exit()
        {
            Managers.Sound.Play("Click");
            // 로비로 돌아가기
            SceneManager.LoadScene(1);
            Managers.Sound.Play("MainBGM", Define.Sound.BGM);
        }
    }
}
