using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VictoryChallenge.KJ.Lobby;
using VictoryChallenge.KJ.Photon;

namespace VictoryChallenge.Scripts.CL
{
    public class LobbyButtonManager : MonoBehaviour
    {
        public Button stageSelectButton;
        public Button gameStartButton;
        public Button leaveLobbyButton;
        public Button[] selectedMapButton;

        private GameObject stageSelectPanel;
        private TextMeshProUGUI stageName;
        private string mapName;
        private Image stageSelectImage;

        void Start()
        {
            stageSelectPanel = GameObject.Find("StageSelect");
            stageName = GameObject.Find("StageName").GetComponent<TextMeshProUGUI>();
            stageSelectImage = stageSelectButton.GetComponent<Image>();
            mapName = "«˜æ– ∏∂∂Û≈Ê";

            stageSelectPanel.SetActive(false);
            stageSelectButton.onClick.AddListener(StageSelect);

            foreach (var mapbutton in selectedMapButton)
            {
                var localbutton = mapbutton;
                var localimage = localbutton.GetComponent<Image>();

                mapbutton.onClick.AddListener(() =>
                {
                    mapName = localbutton.name;
                    switch (mapName)
                    {
                        case "Marathon":
                            stageName.text = "«˜æ– ∏∂∂Û≈Ê";
                            break;
                        case "OnlyOne":
                            stageName.text = "√÷»ƒ¿« 1¿Œ";
                            break;
                        case "Zombie":
                            stageName.text = "¡ª∫Ò";
                            break;
                        case "KimHyeongDon":
                            stageName.text = "±Ë«¸µ∑";
                            break;
                        default:
                            break;
                    }
                    stageSelectImage.sprite = localimage.sprite;
                    stageSelectPanel.SetActive(false);
                });

            }

            gameStartButton.onClick.AddListener(GameScene);
            leaveLobbyButton.onClick.AddListener(() => LobbyLauncher.Instance.LeaveRoom());
        }

        private void Update()
        {
            if (stageSelectPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                stageSelectPanel.SetActive(false);
            }
        }

        void StageSelect()
        {
            stageSelectPanel.SetActive(true);
        }

        void GameScene()
        {
            if (mapName == "Marathon")
                SceneManager.LoadScene(2);
            //if (mapName == "OnlyOne")
              //SceneManager.LoadScene(3); // √÷»ƒ¿«1¿Œ∏ §§
        }
    }
}