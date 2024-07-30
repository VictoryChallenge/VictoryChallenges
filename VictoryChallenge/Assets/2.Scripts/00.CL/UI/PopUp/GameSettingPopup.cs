using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VictoryChallenge.KJ.Room;

namespace VictoryChallenge.Scripts.CL
{ 
    public class GameSettingPopup : UI_Popup
    {
        enum Buttons
        {
            Exit,
        }

        enum Toggles
        {
            Sound,
        }

        enum Sliders
        {
            SoundSlider,
        }

        AudioSource _audiosource;
        AudioSource _audiosourceE;

        void Start()
        {
            Init();
            Cursor.visible = true;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            _audiosource = GameObject.Find("BGM").GetComponent<AudioSource>();
            _audiosourceE = GameObject.Find("Effect").GetComponent<AudioSource>();
            // 슬라이더 값을 오디오 소스의 볼륨 값으로 초기화
            Slider soundSlider = GetSlider((int)Sliders.SoundSlider);
            if (soundSlider != null)
            {
                soundSlider.value = _audiosource.volume;
            }
            else
            {
                Debug.LogError("SoundSlider is null. Please check if it is correctly bound.");
            }
        }

        public override void Init()
        {
            base.Init();
            Bind<Slider>(typeof(Sliders));
            Bind<Button>(typeof(Buttons));
            Bind<Toggle>(typeof(Toggles));

            GetButton((int)Buttons.Exit).gameObject.AddUIEvent((PointerEventData data) => LeaveRoom());
            GetSlider((int)Sliders.SoundSlider).onValueChanged.AddListener(UpdateVolume);
            GetToggle((int)Toggles.Sound).onValueChanged.AddListener(UpdateToggleImage);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                ClosePopupUI();
                Cursor.visible = false;
            }
        }

        void UpdateVolume(float volume)
        {
            _audiosource.volume = volume;
            PlayerPrefs.SetFloat("Volume", volume);  // 볼륨 값 저장
            PlayerPrefs.Save();
        }

        void UpdateToggleImage(bool isOn)
        {
            Sprite sound_on = Resources.Load<Sprite>("Sound-On");
            Sprite sound_off = Resources.Load<Sprite>("Sound-Off");

            Toggle soundToggle = GetToggle((int)Toggles.Sound);
            if (soundToggle != null)
            {
                soundToggle.image.sprite = isOn ? sound_on : sound_off;
                _audiosource.mute = !isOn;
            }
            else
            {
                Debug.LogError("SoundToggle is null. Please check if it is correctly bound.");
            }
        }

        void LeaveRoom()
        {
            RoomMananger.Instance.LeaveRoom();
        }
    }
}
