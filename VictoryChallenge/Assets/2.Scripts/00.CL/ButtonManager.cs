using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VictoryChallenge.KJ.Menu;
using VictoryChallenge.KJ.Photon;

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
        public Button joinButton;
        public Button createRoomButton;
        public Button findRoomButton;
        public Button setRoomButton;
        public Button settingsButton;
        public Button[] exitButtons; // 여러 exitButton을 담을 배열
        public Button exitGameButton;

        public TMP_Dropdown resolutionDropdown;
        public Toggle screenMode;

        private GameObject joinPopup;
        private GameObject settingsPopup;

        private List<Resolution> resolutions;

        void Start()
        {
            // Dropdown 해상도 옵션 초기화 및 관리
            InitializeResolutionOptions();

            // 해상도 변경 dropdown 값 변화에 따른 addlistener
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChange);
            Screen.SetResolution(2560, 1440, true);    // 초기 값 2560x1440, 전체화면

            // 전체화면 or 창화면 토글 값 변화에 따른 addlistener
            screenMode.onValueChanged.AddListener(ScreenModeUpdate);
            screenMode.isOn = (Screen.fullScreenMode == FullScreenMode.FullScreenWindow);

            joinButton.onClick.AddListener(JoinRoom);  // 방 관련 팝업 띄우기
            createRoomButton.onClick.AddListener(() => MenuManager.Instance.OpenMenu("CreateRoom"));  // CreateRoom 메뉴 띄우기
            findRoomButton.onClick.AddListener(() => MenuManager.Instance.OpenMenu("FindRoom"));  // FindRoom 메뉴 띄우기
            setRoomButton.onClick.AddListener(() => PhotonLauncher.Instance.OnJoinedRoom());  // 로비이동.
            settingsButton.onClick.AddListener(SettingOptions);  // Create Room or FindRoom 팝업 띄우기, 메뉴매니저로 관리 안 함
            exitGameButton.onClick.AddListener(ExitGame);  // Settings 팝업 띄우기, 메뉴매니저로 관리 안 함

            // exitButtons 배열에 있는 모든 버튼에 리스너 추가 (창닫기)
            foreach (Button exitButton in exitButtons)
            {
                exitButton.onClick.AddListener(() => ExitMenu(exitButton));
            }

            joinPopup = GameObject.Find("JoinGame");
            joinPopup.SetActive(false);

            settingsPopup = GameObject.Find("Settings");
            settingsPopup.SetActive(false);
        }

        private void Update()
        {
            if (true && Input.GetKeyDown(KeyCode.Escape))
            {

            }
        }

        void JoinRoom()
        {
            joinPopup.SetActive(true);
        }

        void ExitMenu(Button exitButton)
        {
            // 클릭된 버튼의 부모 객체를 비활성화
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

            foreach (var resolution in resolutions)
            {
                string option = resolution.width + " x " + resolution.height;
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
