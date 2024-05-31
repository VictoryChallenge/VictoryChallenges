using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.CL
{
    [System.Serializable]
    public struct Resolution
    {
        public int width;
        public int height;
    }

    public class ButtonManager : MonoBehaviour
    {
        public TMP_Dropdown resolutionDropdown;
        public Toggle screenMode;

        public Button joinButton;
        public Button settingsButton;
        public Button[] exitButtons; // ���� exitButton�� ���� �迭
        public Button exitGameButton;

        private GameObject joinPopup;
        private GameObject settingsPopup;

        private List<Resolution> resolutions;

        void Start()
        {
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChange);
            Screen.SetResolution(2560, 1440, true);

            screenMode.onValueChanged.AddListener(ScreenModeUpdate);
            screenMode.isOn = (Screen.fullScreenMode == FullScreenMode.FullScreenWindow);

            InitializeResolutionOptions();

            joinButton.onClick.AddListener(JoinRoom);
            settingsButton.onClick.AddListener(SettingOptions);
            exitGameButton.onClick.AddListener(ExitGame);

            // exitButtons �迭�� �ִ� ��� ��ư�� ������ �߰�
            foreach (Button exitButton in exitButtons)
            {
                exitButton.onClick.AddListener(() => ExitMenu(exitButton));
            }

            joinPopup = GameObject.Find("JoinGame");
            joinPopup.SetActive(false);
            settingsPopup = GameObject.Find("Settings");
            settingsPopup.SetActive(false);
        }

        void JoinRoom()
        {
            joinPopup.SetActive(true);
        }

        void ExitMenu(Button exitButton)
        {
            // Ŭ���� ��ư�� �θ� ��ü�� ��Ȱ��ȭ
            GameObject parentObject = exitButton.transform.parent.gameObject;
            parentObject.SetActive(false);
        }

        void SettingOptions()
        {
            settingsPopup.SetActive(true);
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

            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();

            foreach (var res in resolutions)
            {
                string option = res.width + " x " + res.height;
                options.Add(option);
            }

            resolutionDropdown.AddOptions(options);
        }

        void OnResolutionChange(int index)
        { 
            if (index >= 0 && index < resolutions.Count)
            {
                Resolution selectedResolution = resolutions[index];
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);
                Debug.Log("Resolution changed to: " + selectedResolution.width + " x " + selectedResolution.height);
            }
        }

        void ScreenModeUpdate(bool isOn)
        {
            if (isOn)
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            else
                Screen.fullScreenMode = FullScreenMode.Windowed;
        }

        void ExitGame()
        { 
            Application.Quit();
        }
    }
}
