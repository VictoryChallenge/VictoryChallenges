using ithappy;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.CL
{ 
    public class GameSceneUI : UI_Scene
    {
        private PhotonView photonView;

        enum TMPs
        { 
            countdown,
        }

        void Start()
        {
            Init();
        }

        TMP_Text text;
        // 캐릭터 이동을 막기 위한 불값 
        public bool isMoving = true;

        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(TMPs));
            Managers.Sound.Play("RunBGM1", Define.Sound.BGM);
            AudioSource _audioSource = GameObject.Find("BGM").GetComponent<AudioSource>();
            _audioSource.volume = 0.4f;

            isMoving = false;
            text = GetTextMeshPro((int)TMPs.countdown);
            text.gameObject.SetActive(false);

            //photonView.RPC("PlayerLoadedScene", RpcTarget.AllBuffered);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameObject go = GameObject.Find("GameSettingPopup");
                if (go != null)
                    return;
                else 
                    Managers.UI.ShowPopupUI<GameSettingPopup>();
            }
        }

        IEnumerator GameStart()
        { 
            yield return new WaitForSeconds(2f);
            text.gameObject.SetActive(true);
            Managers.Sound.Play("Countdown", Define.Sound.Effect);

            text.text = "3";
            yield return new WaitForSeconds(1f);

            text.text = "2";
            yield return new WaitForSeconds(1f);

            text.text = "1";

            isMoving = true;  // 캐릭터 움직임 막을
            
            Rnd_Animation[] objs = FindObjectsOfType<Rnd_Animation>();
            foreach(var obj in objs)
            {
                obj.Active();
            }

            yield return new WaitForSeconds(1f);

            text.text = "달려라~";
            yield return new WaitForSeconds(1f);

            text.gameObject.SetActive(false);
        }
    }
}
