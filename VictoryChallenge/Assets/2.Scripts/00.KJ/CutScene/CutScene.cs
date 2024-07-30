using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

namespace VictoryChallenge.KJ.CutScene
{
    public class CutScene : MonoBehaviour
    {
        private CinemachineVirtualCamera _introCam;
        private PlayableDirector _introTimeline;

        void Start()
        {
            _introCam = GameObject.Find("IntroCam").GetComponent<CinemachineVirtualCamera>();
            _introTimeline = GameObject.Find("IntroTimeline").GetComponent<PlayableDirector>();
            _introTimeline.stopped += OnStopTimeline;
        }

        private void OnStopTimeline(PlayableDirector playableDirector)
        {
            _introCam.enabled = false;
        }
    }

}