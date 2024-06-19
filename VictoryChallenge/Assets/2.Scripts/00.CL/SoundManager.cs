using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Scripts.CL
{ 
    public class SoundManager : MonoBehaviour
    {
        AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
        Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

        public void Init()
        { 
            GameObject root = GameObject.Find("Sound");
            if (root != null)
            {
                root = new GameObject { name = "Sound" };
                Object.DontDestroyOnLoad(root);
            }

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));  // BGM, SOUND
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }
        }

        void Start()
        {
        
        }

        void Update()
        {
        
        }
    }
}
