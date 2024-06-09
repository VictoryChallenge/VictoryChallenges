using GSpawn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.Scripts.CL;

namespace VictoryChallenge.Scripts.CL
{ 
    public class UI_Popup : UI_Base
    {
        public virtual void init()
        {
            Managers.UI.SetCanvas(gameObject, true);
        }

        public virtual void ClosePopupUI()
        {
            Managers.UI.ClosePopupUI(this);
        }
    }
}