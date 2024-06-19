using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Scripts.CL
{ 
    /// <summary>
    /// UI �̺�Ʈ�鿡 ���� ������ �������� �����س��� Ŭ����
    /// </summary>
    public class Define
    {
        public enum UIEvent
        {
            Click,
            Drag,
        }

        public enum MouseEvent
        {
            Press,
            Click,
        }

        public enum CameraMode
        {
            Quarterview,
        }

        //public enum Scene
        //{
        //    Unknown, // ����Ʈ
        //    Login, // �α��� ȭ�� ��
        //    Lobby, // �κ� ��
        //    Game, // �ΰ��� ��
        //}
    }
}