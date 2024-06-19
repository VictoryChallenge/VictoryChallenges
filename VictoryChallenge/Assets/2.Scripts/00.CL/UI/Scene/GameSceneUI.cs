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

        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(TMPs));

            text = GetTextMeshPro((int)TMPs.countdown);

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
            yield return null;

            text.text = "3";
            yield return new WaitForSeconds(1f);

            text.text = "2";
            yield return new WaitForSeconds(1f);

            text.text = "1";
            yield return new WaitForSeconds(1f);

            text.gameObject.SetActive(false);
        }
    }
}
