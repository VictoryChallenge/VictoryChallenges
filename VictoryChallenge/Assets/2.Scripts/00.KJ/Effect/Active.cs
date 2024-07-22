using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

namespace VictoryChallenge.KJ.Effect
{
    public class Active : MonoBehaviour
    {
        private CinemachineVirtualCamera _resultCam;
        private CinemachineVirtualCamera _playercam;
        private PlayableDirector _resultTimeline;

        protected virtual void Start()
        {
            _resultCam = GameObject.Find("ResultCam").GetComponent<CinemachineVirtualCamera>();
            _resultTimeline = GameObject.Find("ResultTimeline").GetComponentInParent<PlayableDirector>();

            _playercam.enabled = false;
            _resultTimeline.stopped += OnStopTimeLine;
        }

        private void OnStopTimeLine(PlayableDirector director)
        {
            _resultCam.enabled = false;
        }

    }
}

