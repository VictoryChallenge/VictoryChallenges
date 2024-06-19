using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.CL
{ 
    public class GameSceneUI : UI_Scene
    {
        enum TMPs
        { 
            countdown,
        }

        void Start()
        {
            Init();
        }

        TMP_Text text;
        // ĳ���� �̵��� ���� ���� �Ұ� 
        public bool isMoving = true;

        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(TMPs));

            isMoving = false;
            text = GetTextMeshPro((int)TMPs.countdown);
            text.gameObject.SetActive(false);

            StartCoroutine(GameStart());
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
            yield return new WaitForSeconds(1f);

            isMoving = true;

            text.text = "�޷���~";
            yield return new WaitForSeconds(1f);


            text.gameObject.SetActive(false);
        }
    }
}
