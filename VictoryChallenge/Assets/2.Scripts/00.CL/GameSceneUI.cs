using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VictoryChallenge.Scripts.CL
{ 
    public class GameSceneUI : MonoBehaviour
    {
        private TextMeshProUGUI _countdown;

        void Start()
        {
            _countdown = GameObject.Find("countdown").GetComponent<TextMeshProUGUI>();
            StartCoroutine(GameStart());
        }

        void Update()
        {
        
        }

        IEnumerator GameStart()
        { 
            yield return null;

            _countdown.text = "3";
            yield return new WaitForSeconds(1f);

            _countdown.text = "2";
            yield return new WaitForSeconds(1f);

            _countdown.text = "1";
            yield return new WaitForSeconds(1f);

            _countdown.gameObject.SetActive(false);
        }
    }
}
