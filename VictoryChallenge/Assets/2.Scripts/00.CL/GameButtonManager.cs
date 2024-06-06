using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace VictoryChallenge.Scripts.CL
{ 
    public class GameButtonManager : MonoBehaviour
    {
        public Toggle sound;
        public Button exit;
        public Sprite sound_on;
        public Sprite sound_off;

        private GameObject _pause; 

        void Start()
        {
            _pause = GameObject.Find("Pause");
            _pause.SetActive(false);
            // 초기 토글 상태에 따른 이미지 설정
            UpdateToggleImage(sound.isOn);

            exit.onClick.AddListener(ReturnMain);

            // 토글 값이 변경될 때 호출될 콜백 설정
            sound.onValueChanged.AddListener(UpdateToggleImage);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                _pause.SetActive(!_pause.activeSelf);
            }        
        }

        void ReturnMain()
        {
            // 메인씬으로가기
            SceneManager.LoadScene(0);
        }

        void UpdateToggleImage(bool isOn)
        {
            sound.image.sprite = isOn ? sound_on : sound_off;
        }
    }
}
