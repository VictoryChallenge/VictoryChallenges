using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VictoryChallenge.KJ.Photon;
using VictoryChallenge.KJ.Room;
using VictoryChallenge.KJ.Lobby;

namespace VictoryChallenge.Scripts.CL
{ 
    public class LobbySceneUI : UI_Scene
    {
        enum Buttons
        { 
            StageSelectButton,
            GameStart,
            LeaveLobby,
        }

        enum TMPs
        { 
            StageName,
            ReadyOrStart,
        }

        enum Images
        {
            PlayerListContent,
        }

        private Image stageSelectButtonImage;

        void Start()
        {
            Init();
            Managers.UI.ShowSceneUI<UI_Scene>("ChatPrefabs");
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<Image>(typeof(Images));
            Bind<TextMeshProUGUI>(typeof(TMPs));

            stageSelectButtonImage = GetButton((int)Buttons.StageSelectButton).GetComponent<Image>();

            PhotonSub.Instance._text = GetTextMeshPro((int)TMPs.ReadyOrStart);
            PhotonSub.Instance._button = GetButton((int)Buttons.GameStart);
            PhotonSub.Instance.UpdateButtonText();
            GetButton((int)Buttons.GameStart).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            GetButton((int)Buttons.StageSelectButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
            GetButton((int)Buttons.LeaveLobby).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 3));

            //if (PlayerList.Instance == null)
            //{
            //    Debug.LogError("PlayerList.Instance is null");
            //}
            //else
            //{
            //    var playerListContentImage = GetImage((int)Images.PlayerListContent);
            //    if (playerListContentImage == null)
            //    {
            //        Debug.LogError("playerListContentImage is null");
            //    }
            //    else
            //    {
            //        PlayerList.Instance.playerListContent = playerListContentImage.gameObject;
            //        Debug.Log(playerListContentImage.name);
            //    }
            //}
        }

        public void OnButtonClicked(PointerEventData data, int a)
        {
            switch (a)
            {
                case 1:
                    Debug.Log("1");
                    break;
                case 2:
                    var stageSelectPopup = Managers.UI.ShowPopupUI<StageSelectPopup>();
                    stageSelectPopup.OnStageSelected += UpdateStageSelectTextSprite; // �̺�Ʈ ����
                    break;
                case 3:
                    Debug.Log("3");
                    LeftLobby();
                    break;
                default:
                    Debug.LogWarning("Unhandled action: " + a);
                    break;
            }
        }

        private void UpdateStageSelectTextSprite(Sprite newSprite, string name)
        {
            if (newSprite != null)
                stageSelectButtonImage.sprite = newSprite;
            if (name != null)
                GetTextMeshPro((int)TMPs.StageName).text = name;

            PhotonView photonView = GetComponent<PhotonView>();
            // �̹��� �� �ؽ�Ʈ ������ ��� Ŭ���̾�Ʈ�� ����
            photonView.RPC("RPC_UpdateStageSelectTextSprite", RpcTarget.All, newSprite.name, name);
        }

        [PunRPC]
        private void RPC_UpdateStageSelectTextSprite(string spriteName, string name)
        {
            // Resources �������� ��������Ʈ �ε�
            Sprite sprite = Resources.Load<Sprite>(spriteName);
            if (sprite != null)
            {
                stageSelectButtonImage.sprite = sprite;
            }
            else
            {
                Debug.LogError("��������Ʈ�� �ε��� �� �����ϴ�: " + spriteName);
            }

            GetTextMeshPro((int)TMPs.StageName).text = name;
        }

        public void LeftLobby()
        {
            RoomMananger.Instance.LeaveRoom();
        }

        
    }
}
