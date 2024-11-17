using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    public class SoundManager
    {
        AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
        Dictionary<string, AudioClip> _audioclips = new Dictionary<string, AudioClip>();
        public void Init()
        {
            GameObject go = GameObject.Find("@Sound");
            if (go == null)
            {
                go = new GameObject { name = "@Sound" };
                Object.DontDestroyOnLoad(go);

                string[] _soundNames = System.Enum.GetNames(typeof(Define.Sound));
                for (int i = 0; i < _soundNames.Length - 1; i++)
                {
                    GameObject go2 = new GameObject { name = _soundNames[i] };
                    _audioSources[i] = go2.AddComponent<AudioSource>();
                    go2.transform.parent = go.transform;
                }


                _audioSources[(int)Define.Sound.Bgm].loop = true;
            }
        }

        public void Clear()
        {
            foreach (AudioSource audioSource in _audioSources)
            {
                audioSource.clip = null;
                audioSource.Stop();
            }
            _audioclips.Clear();
        }

        public void Play(Define.Sound type)
        {
            Play(type.ToString(), Define.Sound.Effect);
        }

        public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
        {
            AudioClip audioClip = GetOrAddAudioClip(path, type);
            Play(audioClip, type, pitch);
        }

        public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
        {
            if (audioClip == null)
            {
                Debug.Log("No Clip");
                return;
            }
            if (type == Define.Sound.Bgm)
            {
                AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
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

        AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
        {
            if (path.Contains("Sounds/") == false)
                path = $"Sounds/{path}";

            AudioClip audioClip = null;

            if (type == Define.Sound.Bgm)
            {
                audioClip = ResourceManager.Instance.Load<AudioClip>(path);
            }
            else
            {
                if (_audioclips.TryGetValue(path, out audioClip) == false)
                {
                    audioClip = ResourceManager.Instance.Load<AudioClip>(path);
                    _audioclips.Add(path, audioClip);
                }
            }

            if (audioClip == null)
                Debug.Log($"AudioClip Missing ! {path}");


            return audioClip;
        }

        public void ChangeVolume(Define.Sound type, float value)
        {
            if (type == Define.Sound.Bgm)
            {
                AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
                audioSource.volume = value;
            }
            else
            {
                AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
                audioSource.volume = value;
            }
        }

    }

}
