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
        }

        public void SceneLoad()
        {
            Managers.Sound.Play("Click", Define.Sound.Effect);
            SceneManager.LoadScene(1);
        }
    }
}
