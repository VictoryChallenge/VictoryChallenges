using ithappy;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using VictoryChallenge.Controllers.Player;

namespace VictoryChallenge.Scripts.CL
{ 
    public class GameSceneUI : UI_Scene
    {
        private PhotonView photonView;

        enum TMPs
        { 
            countdown,
        }

        TMP_Text text;
        // 캐릭터 이동을 막기 위한 불값 
        private bool isMoving;

        void Start()
        {
            Init();
        }

        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(TMPs));
            Managers.Sound.Play("RunBGM1", Define.Sound.BGM);
            AudioSource _audioSource = GameObject.Find("BGM").GetComponent<AudioSource>();
            _audioSource.volume = 0.4f;

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
            Controllers.Player.CharacterController[] cc = GameObject.FindObjectsOfType<Controllers.Player.CharacterController>();
            foreach (Controllers.Player.CharacterController c in cc)
            {
                c.isKeyActive = false;
                Debug.Log($"{c.isKeyActive} + 펄스");
                yield return c.StartCoroutine(c.C_IntroCutSceneStart());
            }

            yield return new WaitForSeconds(2f);
            text.gameObject.SetActive(true);
            Managers.Sound.Play("Countdown", Define.Sound.Effect);

            text.text = "3";
            yield return new WaitForSeconds(1f);

            text.text = "2";
            yield return new WaitForSeconds(1f);

            text.text = "1";     
            
            Rnd_Animation[] objs = FindObjectsOfType<Rnd_Animation>();
            foreach(var obj in objs)
            {
                obj.Active();
            }

            yield return new WaitForSeconds(1f);

            foreach (Controllers.Player.CharacterController c in cc)
            {
                c.isKeyActive = true;
                Debug.Log($"{c.isKeyActive} + 트루");
            }

            text.text = "달려라~";
            yield return new WaitForSeconds(1f);

            text.gameObject.SetActive(false);
        }
    }
}
