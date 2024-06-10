using GSpawn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VictoryChallenge.Scripts.CL
{
    public class UI_Scene : UI_Base
    {
        public override void Init()
        {
            Managers.UI.SetCanvas(gameObject, false);
        }
    }
}
