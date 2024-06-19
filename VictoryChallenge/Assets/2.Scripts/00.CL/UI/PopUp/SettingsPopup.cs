using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.CL
{
    public class SettingsPopup : UI_Popup
    {
        enum Sliders
        {
            Slider,
        }

        enum Buttons
        { 
            exitButton,
        }

        enum Dropdowns
        {
            Dropdown,
        }

        enum Toggles
        {
            ScreenModeToggle,
        }

        private List<Resolution> resolutions;

        void Start()
        {
            Init();
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;

            LoadSettings();
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<Slider>(typeof(Sliders));
            Bind<TMP_Dropdown>(typeof(Dropdowns));
            Bind<Toggle>(typeof(Toggles));

            InitializeResolutionOptions();
            GetButton((int)Buttons.exitButton).onClick.AddListener(ClosePopup);
            GetDropdown((int)Dropdowns.Dropdown).onValueChanged.AddListener(OnResolutionChange);
            GetToggle((int)Toggles.ScreenModeToggle).onValueChanged.AddListener(ScreenModeUpdate);
            GetToggle((int)Toggles.ScreenModeToggle).isOn = (Screen.fullScreenMode == FullScreenMode.FullScreenWindow);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePopupUI();
            }
        }

        void ClosePopup()
        {
            Managers.Sound.Play("Click", Define.Sound.Effect);
            ClosePopupUI();
        }

        void InitializeResolutionOptions()
        {
            resolutions = new List<Resolution>()
            {
                new Resolution { width = 2560, height = 1440 },
                new Resolution { width = 1920, height = 1080 },
                new Resolution { width = 1280, height = 720 },
                new Resolution { width = 800, height = 600 }
            };

            GetDropdown((int)Dropdowns.Dropdown).ClearOptions();
            List<string> options = new List<string>();

            foreach (var resolution in resolutions)
            {
                string option = resolution.width + " x " + resolution.height;
                options.Add(option);
            }

            GetDropdown((int)Dropdowns.Dropdown).AddOptions(options);
        }

        void ScreenModeUpdate(bool isOn)
        {
            Managers.Sound.Play("Click", Define.Sound.Effect);
            Screen.fullScreenMode = isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            PlayerPrefs.SetInt("FullScreen", isOn ? 1 : 0); // 1 for fullscreen, 0 for windowed
        }

        void OnResolutionChange(int index)
        {
            if (index >= 0 && index < resolutions.Count)
            {
                Resolution selectedResolution = resolutions[index];
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);
                PlayerPrefs.SetInt("ResolutionIndex", index);
                Debug.Log("Resolution changed to: " + selectedResolution.width + " x " + selectedResolution.height);
            }
        }

        void LoadSettings()
        {
            // Load full screen setting
            bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1; // Default to fullscreen
            Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            GetToggle((int)Toggles.ScreenModeToggle).isOn = isFullScreen;

            // Load resolution setting
            int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0); // Default to the first resolution
            if (resolutionIndex >= 0 && resolutionIndex < resolutions.Count)
            {
                Resolution selectedResolution = resolutions[resolutionIndex];
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);
                GetDropdown((int)Dropdowns.Dropdown).value = resolutionIndex;
                GetDropdown((int)Dropdowns.Dropdown).RefreshShownValue();
            }
        }
    }
}
