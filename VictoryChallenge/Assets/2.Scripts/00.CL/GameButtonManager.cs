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
        public Slider volumeslider;
        public Button exit;
        public Sprite sound_on;
        public Sprite sound_off;
        public AudioSource _audiosource;

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

            if (_audiosource == null)
            {
                Debug.LogError("AudioSource not assigned");
            }
            else
            {
                // �ʱ� ��� ���¿� ���� �̹��� ����
                UpdateToggleImage(sound.isOn);

                float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
                volumeslider.value = savedVolume;
                _audiosource.volume = savedVolume;

                Debug.Log("Initial volume set to: " + savedVolume);
            }

            volumeslider.onValueChanged.AddListener(UpdateVolume);
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
            _audiosource.mute = !isOn;
        }

        void UpdateVolume(float volume)
        {
            _audiosource.volume = volume;
            PlayerPrefs.SetFloat("Volume", volume);  // ���� �� ����
            PlayerPrefs.Save();

            // Debug.Log�� ���� �� Ȯ��
            Debug.Log("Volume updated: " + volume + "dd" + _audiosource.volume);
        }
    }
}
