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
            MaxCount
        }

        public enum MouseEvent
        {
            Press,
            Click,
            MaxCount
        }

        public enum Sound
        {
            BGM,
            Effect,
            MaxCount
        }

        //public enum Scene
        //{
        //    Unknown, // 디폴트
        //    Login, // 로그인 화면 씬
        //    Lobby, // 로비 씬
        //    Game, // 인게임 씬
        //}
    }
}