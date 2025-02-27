using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;


namespace Client
{
    public class SoundManager : Singleton<SoundManager>
    {
        private const string BGMvolKey = "BGMvol";
        private const string SFXvolKey = "SFXvol";

        AudioSource[] _audioSources = new AudioSource[(int)SystemEnum.eSound.MaxCount];

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

                string[] _soundNames = System.Enum.GetNames(typeof(SystemEnum.eSound));
                for (int i = 0; i < _soundNames.Length - 1; i++)
                {
                    GameObject go2 = new GameObject { name = _soundNames[i] };
                    _audioSources[i] = go2.AddComponent<AudioSource>();
                    go2.transform.parent = go.transform;
                }


                _audioSources[(int)SystemEnum.eSound.BGM_Main].loop = true;
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

        public void Play(SystemEnum.eSound type)
        {
            Play(type.ToString(), type);
        }

        public void Play(string path, SystemEnum.eSound type, float pitch = 1.0f)
        {
            AudioClip audioClip = GetOrAddAudioClip(path, type);
            Play(audioClip, type, pitch);
        }

        public void Play(AudioClip audioClip, SystemEnum.eSound type, float pitch = 1.0f)
        {
            if (audioClip == null)
            {
                Debug.Log("No Clip");
                return;
            }
            if (type.ToString().StartsWith("BGM"))
            {
                AudioSource audioSource = _audioSources[(int)SystemEnum.eSound.BGM_Main];
                if (audioSource.isPlaying)
                    audioSource.Stop();

                audioSource.pitch = pitch;
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                AudioSource audioSource = _audioSources[(int)SystemEnum.eSound.SFX_Positive];
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(audioClip);
            }

        }

        AudioClip GetOrAddAudioClip(string path, SystemEnum.eSound type)
        {
            if (!path.Contains("Sounds/"))
                path = $"Sounds/{path}";

            AudioClip audioClip = null;

            if (type.ToString().StartsWith("BGM"))
            {
                audioClip = ResourceManager.Instance.Load<AudioClip>(path);
            }
            else
            {
                if (!_audioclips.TryGetValue(path, out audioClip))
                {
                    audioClip = ResourceManager.Instance.Load<AudioClip>(path);
                    _audioclips.Add(path, audioClip);
                }
            }

            if (audioClip == null)
                Debug.Log($"AudioClip Missing ! {path}");


            return audioClip;
        }

        public void ChangeVolume(bool isBGM, float value)
        {
            if (isBGM)
            {
                AudioSource audioSource = _audioSources[(int)SystemEnum.eSound.BGM_Main];
                audioSource.volume = value;
                PlayerPrefs.SetFloat(BGMvolKey, value);
                PlayerPrefs.Save();
            }
            else
            {
                // 모든 SFX AudioSource 볼륨 적용
                for (eSound sounds = eSound.BGM_Main; sounds < eSound.MaxCount; sounds++)
                {
                    if (sounds.ToString().StartsWith("SFX"))
                    {
                        _audioSources[(int)sounds].volume = value;
                        Debug.Log(_audioSources[(int)sounds].volume);
                    }
                }
                PlayerPrefs.SetFloat(SFXvolKey, value);
                PlayerPrefs.Save();
            }
        }

    }

}
