using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Scripts.CL
{ 
    /// <summary>
    /// UI 이벤트들에 대한 열거형 변수들을 정리해놓은 클래스
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
    }
}