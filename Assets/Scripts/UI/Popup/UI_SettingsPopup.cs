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

        enum Sliders
        {
            Slider_BGM,
            Slider_SFX,
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<Slider>(typeof(Sliders));

            BindButton();
            BindSlider();
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_Credit).gameObject, OnClickCreditBtn);
        }

        void BindSlider()
        {
            Get<Slider>((int)Sliders.Slider_BGM).onValueChanged.AddListener(ChangeBGM);
            Get<Slider>((int)Sliders.Slider_SFX).onValueChanged.AddListener(ChangeSFX);
            Get<Slider>((int)Sliders.Slider_BGM).value = LoadBGMVolume();
            Get<Slider>((int)Sliders.Slider_SFX).value = LoadSFXVolume();

        }

        #region 버튼 이벤트
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
        #endregion

        #region 볼륨 설정
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
        #endregion
    }
}
