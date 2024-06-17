using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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

        void Start()
        {
            Init();
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            _audiosource = GameObject.FindAnyObjectByType<AudioSource>().GetComponent<AudioSource>();
        }

        public override void Init()
        {
            base.Init();
            Bind<Slider>(typeof(Sliders));
            Bind<Button>(typeof(Buttons));
            Bind<Toggle>(typeof(Toggles));

            GetSlider((int)Sliders.SoundSlider).onValueChanged.AddListener(UpdateVolume);
            GetToggle((int)Toggles.Sound).onValueChanged.AddListener(UpdateToggleImage);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                ClosePopupUI();
            }
        }

        void UpdateVolume(float volume)
        {
            _audiosource.volume = volume;
            PlayerPrefs.SetFloat("Volume", volume);  // º¼·ý °ª ÀúÀå
            PlayerPrefs.Save();

            // Debug.Log·Î º¼·ý °ª È®ÀÎ
            Debug.Log("Volume updated: " + volume + "º¼·ý" + _audiosource.volume);
        }

        void UpdateToggleImage(bool isOn)
        {
            Sprite sound_on = Resources.Load<Sprite>("Sound-On");
            Sprite sound_off = Resources.Load<Sprite>("Sound-Off");

            GetToggle((int)Toggles.Sound).image.sprite = isOn ? sound_on : sound_off;
            _audiosource.mute = !isOn;
        }
    }
}
