using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    public class UI_SettingsPopup : UI_Popup
    {
        private const string BGMvolKey = "BGMvol";
        private const string SFXvolKey = "SFXvol";

        enum Buttons
        {
            Panel,
            BTN_Credit,
        }

        public Slider BGMslider;
        public Slider SFXslider;

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            BindButton();

            BGMslider = GameObject.Find("Slider_BGM").GetComponent<Slider>();
            SFXslider = GameObject.Find("Slider_SFX").GetComponent<Slider>();

            if (BGMslider == null)
            {
                Debug.LogError("BGM 슬라이더를 찾을 수 없습니다!");
            }
            if (SFXslider == null)
            {
                Debug.LogError("SFX 슬라이더를 찾을 수 없습니다!");
            }

            // BindSlider();
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_Credit).gameObject, OnClickCreditBtn);
        }

        void BindSlider()
        {
            BGMslider.onValueChanged.AddListener(ChangeBGM);
            SFXslider.onValueChanged.AddListener(ChangeSFX);
            BGMslider.value = LoadBGMVolume();
            SFXslider.value = LoadSFXVolume();

        }

        void OnClickPanel(PointerEventData evt)
        {
            Debug.Log("판넬 누름..");
            ClosePopupUI();
        }

        void OnClickCreditBtn(PointerEventData evt)
        {
            Debug.Log("크레딧 버튼 클릭");
            UI_Manager.Instance.ShowPopupUI<UI_CreditPopup>();
        }

        void ChangeBGM(float value)
        {
            SoundManager.Instance.ChangeVolume(Define.Sound.BGM, value);
            PlayerPrefs.SetFloat(BGMvolKey, value);
            PlayerPrefs.Save();
        }
        void ChangeSFX(float value)
        {
            SoundManager.Instance.ChangeVolume(Define.Sound.SFX, value);
            PlayerPrefs.SetFloat(SFXvolKey, value);
            PlayerPrefs.Save();
        }
        public float LoadBGMVolume()
        {
            if (PlayerPrefs.HasKey(BGMvolKey))
            {
                return PlayerPrefs.GetFloat(BGMvolKey);
            }
            else
            {
                // default
                return 1.0f;
            }
        }
        public float LoadSFXVolume()
        {
            if (PlayerPrefs.HasKey(SFXvolKey))
            {
                return PlayerPrefs.GetFloat(SFXvolKey);
            }
            else
            {
                // default
                return 1.0f;
            }
        }
    }
}
