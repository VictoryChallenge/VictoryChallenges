using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VictoryChallenge.Scripts.CL
{
    public class LobbyButtonManager : MonoBehaviour
    {
        public Button stageSelect;
        public Button gameStart;
        public Button[] selectedMapButton;
        public Image stageSelectImage;

        private GameObject stageSelectPanel;
        private TextMeshProUGUI stageName;
        private string mapName;

        void Start()
        {
            stageSelectPanel = GameObject.Find("StageSelect");
            stageName = GameObject.Find("StageName").GetComponent<TextMeshProUGUI>();
            stageSelectImage = stageSelect.GetComponent<Image>();
            mapName = "���� ������";

            stageSelectPanel.SetActive(false);
            stageSelect.onClick.AddListener(StageSelect);

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
                            stageName.text = "���� ������";
                            break;
                        case "OnlyOne":
                            stageName.text = "������ 1��";
                            break;
                        case "Zombie":
                            stageName.text = "����";
                            break;
                        case "KimHyeongDon":
                            stageName.text = "������";
                            break;
                        default:
                            break;
                    }
                    stageSelectImage.sprite = localimage.sprite;
                    stageSelectPanel.SetActive(false);
                });

            }
            gameStart.onClick.AddListener(GameScene);
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
              //SceneManager.LoadScene(3); // ������1�θʤ�
        }
    }
}