using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Scripts.CL
{ 
    public class SoundManager
    {
        AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
        Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

        public void Init()
        { 
            GameObject root = GameObject.Find("Sound");
            if (root == null)
            {
                root = new GameObject { name = "Sound" };
                Object.DontDestroyOnLoad(root);

                string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));  // BGM, SOUND
                for (int i = 0; i < soundNames.Length - 1; i++)
                {
                    GameObject go = new GameObject { name = soundNames[i] };
                    _audioSources[i] = go.AddComponent<AudioSource>();
                    go.transform.parent = root.transform;
                }

                _audioSources[(int)Define.Sound.BGM].loop = true;
            }
        }

        public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
        {
            if (audioClip == null)
                return;

            if (type == Define.Sound.BGM)
            {
                AudioSource audioSource = _audioSources[(int)Define.Sound.BGM];
                if (audioSource.isPlaying)
                    audioSource.Stop();

                audioSource.pitch = pitch;
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            { 
                AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(audioClip);
            }
        }

        public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
        { 
            AudioClip audioClip = GetOrAddAudioClip(path, type);
            Play(audioClip, type, pitch);
            if (audioClip == null)
                Debug.Log("�����Ŭ����");
        }

        AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
        {
            if (path.Contains ("Sounds/") == false)
                path = $"Sounds/{path}";    // sounds���� �ȿ� ����� �� �ֵ���

            AudioClip audioClip = null;

            // BGM ������� Ŭ�� ���̱�
            if (type == Define.Sound.BGM)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
            }
            // Effect ȿ���� Ŭ�� ���̱�
            else
            {
                if (_audioClips.TryGetValue(path, out audioClip) == false)
                { 
                    audioClip = Managers.Resource.Load<AudioClip>(path);
                    _audioClips.Add(path, audioClip);
                }
            }

            if (audioClip == null)
                Debug.Log($"AudioClip Missing : {path}");

            return audioClip;
        }

        public void Clear()
        {
            // ����� ���� ��ž, ���� ����
            foreach (AudioSource audioSource in _audioSources)
            {
                audioSource.clip = null;
                audioSource.Stop();
            }

            _audioClips.Clear();
        }
    }
}
