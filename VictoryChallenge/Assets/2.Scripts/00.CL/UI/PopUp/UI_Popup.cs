using GSpawn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.Scripts.CL;

namespace VictoryChallenge.Scripts.CL
{
    public class UI_Popup : UI_Base
    {
        public override void Init()
        {
            Managers.UI.SetCanvas(gameObject, true);
        }

        public virtual void ClosePopupUI()  // �˾��̴ϱ� ���� ĵ����(Scene)�� �ٸ��� �ݴ°� �ʿ�
        {
            Managers.UI.ClosePopupUI(this);
        }

        private void OnDestroy()
        {
            ClosePopupUI();
        }
    }
}