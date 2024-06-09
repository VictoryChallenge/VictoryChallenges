using GSpawn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VictoryChallenge.Scripts.CL
{
    public class UI_Scene : UI_Base
    {
        public virtual void init()
        {
            Managers.UI.SetCanvas(gameObject, false);
        }
    }
}
