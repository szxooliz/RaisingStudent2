using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.Define;


namespace Client
{
    public class SoundManager : Singleton<SoundManager>
    {
        AudioSource[] _audioSources = new AudioSource[(int)Define.eSound.MaxCount];

        private float[] _volume = new float[(int)eSound.MaxCount];

        Dictionary<string, AudioClip> _audioclips = new Dictionary<string, AudioClip>();

        #region 생성자 
        private SoundManager()
        {
            for (eSound sounds = 0; sounds < eSound.MaxCount; sounds++)
            {
                _volume[(int)sounds] = PlayerPrefs.GetFloat($"{sounds}Volume", 1f);
            }
        }
        #endregion 생성자 

        public override void Init()
        {
            GameObject go = GameObject.Find("@Sound");
            if (go == null)
            {
                go = new GameObject { name = "@Sound" };
                Object.DontDestroyOnLoad(go);

                string[] _soundNames = System.Enum.GetNames(typeof(Define.eSound));
                for (int i = 0; i < _soundNames.Length - 1; i++)
                {
                    GameObject go2 = new GameObject { name = _soundNames[i] };
                    _audioSources[i] = go2.AddComponent<AudioSource>();
                    go2.transform.parent = go.transform;
                }


                _audioSources[(int)Define.eSound.BGM].loop = true;
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

        public void Play(Define.eSound type)
        {
            Play(type.ToString(), Define.eSound.SFX);
        }

        public void Play(string path, Define.eSound type = Define.eSound.SFX, float pitch = 1.0f)
        {
            AudioClip audioClip = GetOrAddAudioClip(path, type);
            Play(audioClip, type, pitch);
        }

        public void Play(AudioClip audioClip, Define.eSound type = Define.eSound.SFX, float pitch = 1.0f)
        {
            if (audioClip == null)
            {
                Debug.Log("No Clip");
                return;
            }
            if (type == Define.eSound.BGM)
            {
                AudioSource audioSource = _audioSources[(int)Define.eSound.BGM];
                if (audioSource.isPlaying)
                    audioSource.Stop();

                audioSource.pitch = pitch;
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                AudioSource audioSource = _audioSources[(int)Define.eSound.SFX];
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(audioClip);
            }

        }

        AudioClip GetOrAddAudioClip(string path, Define.eSound type = Define.eSound.SFX)
        {
            if (path.Contains("Sounds/") == false)
                path = $"Sounds/{path}";

            AudioClip audioClip = null;

            if (type == Define.eSound.BGM)
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

        public void ChangeVolume(Define.eSound type, float value)
        {
            if (type == Define.eSound.BGM)
            {
                AudioSource audioSource = _audioSources[(int)Define.eSound.BGM];
                audioSource.volume = value;
            }
            else
            {
                AudioSource audioSource = _audioSources[(int)Define.eSound.SFX];
                audioSource.volume = value;
            }
        }

    }

}
