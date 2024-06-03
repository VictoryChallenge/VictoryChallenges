using Photon.Pun;
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
            mapName = "Ç÷¾Ð ¸¶¶óÅæ";

            stageSelectPanel.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
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
                            stageName.text = "Ç÷¾Ð ¸¶¶óÅæ";
                            break;
                        case "OnlyOne":
                            stageName.text = "ÃÖÈÄÀÇ 1ÀÎ";
                            break;
                        case "Zombie":
                            stageName.text = "Á»ºñ";
                            break;
                        case "OnlyUp":
                            stageName.text = "¿Â¸®¾÷";
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
                PhotonNetwork.LoadLevel(2);
            //if (mapName == "OnlyOne")
                //PhotonNetwork.LoadLevel(3);
        }
    }
}