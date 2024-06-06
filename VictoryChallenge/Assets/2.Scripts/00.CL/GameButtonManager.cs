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
            // �ʱ� ��� ���¿� ���� �̹��� ����
            UpdateToggleImage(sound.isOn);

            exit.onClick.AddListener(ReturnMain);

            // ��� ���� ����� �� ȣ��� �ݹ� ����
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
            // ���ξ����ΰ���
            SceneManager.LoadScene(0);
        }

        void UpdateToggleImage(bool isOn)
        {
            sound.image.sprite = isOn ? sound_on : sound_off;
        }
    }
}
