using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VictoryChallenge.KJ.Room;

namespace VictoryChallenge.Scripts.CL
{ 
    public class CustomizeSceneUI : UI_Scene
    {
        enum Buttons
        { 
            LeftChoice,
            RightChoice,
            LeaveRoomButton
        }

        void Start()
        {
            Init();
        }

        public override void Init()
        {
            Bind<Button>(typeof(Buttons));

            //Managers.UI.ShowPopupUI<UI_Popup>("CustomButtonPopup");
            //GetButton((int)Buttons.LeftChoice).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 1));
            //GetButton((int)Buttons.RightChoice).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 2));
            //GetButton((int)Buttons.LeaveRoomButton).gameObject.AddUIEvent((PointerEventData data) => OnButtonClicked(data, 3));
        }

        //public void OnButtonClicked(PointerEventData data, int a)
        //{
        //    switch (a)
        //    {
        //        case 1:
        //            Debug.Log("왼쪽버튼.");
        //            break;
        //        case 2:
        //            Debug.Log("오른쪽버튼.");
        //            break;
        //        case 3:
        //            Debug.Log("로비.");
        //            break;
        //        default:
        //            Debug.LogWarning("Unhandled action: " + a);
        //            break;
        //    }
        //}

        public void SceneLoad()
        {
            SceneManager.LoadScene(1);
        }
    }
}
