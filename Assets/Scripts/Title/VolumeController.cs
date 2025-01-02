using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider bgmSlider; // 배경 음악 슬라이더
    public Slider sfxSlider; // 효과음 슬라이더

    private const string BGMvolKey = "BGMvol"; // PlayerPrefs 키
    private const string SFXvolKey = "SFXvol"; // PlayerPrefs 키

    void Start()
    {
        // 슬라이더 초기값 설정
        InitializeSliders();
    }

    private void InitializeSliders()
    {
        // 저장된 값을 로드하거나 기본값으로 초기화
        bgmSlider.value = LoadVolume(BGMvolKey, 0.8f); // 기본값 0.8
        sfxSlider.value = LoadVolume(SFXvolKey, 0.8f);

        // 슬라이더 값 변경 이벤트 연결
        bgmSlider.onValueChanged.AddListener(ChangeBGM);
        sfxSlider.onValueChanged.AddListener(ChangeSFX);
    }

    private void ChangeBGM(float value)
    {
        Debug.Log($"BGM 볼륨 변경: {value}");
        PlayerPrefs.SetFloat(BGMvolKey, value); // 값 저장
        PlayerPrefs.Save();
    }

    private void ChangeSFX(float value)
    {
        Debug.Log($"SFX 볼륨 변경: {value}");
        PlayerPrefs.SetFloat(SFXvolKey, value); // 값 저장
        PlayerPrefs.Save();
    }

    private float LoadVolume(string key, float defaultValue)
    {
        // PlayerPrefs에 저장된 값을 가져오거나 기본값 반환
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : defaultValue;
    }
}

