using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

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
            BTN_X,
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
            BindEvent(GetButton((int)Buttons.BTN_X).gameObject, OnClickXBtn);
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
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        void OnClickCreditBtn(PointerEventData evt)
        {
            Debug.Log("크레딧 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);
            UI_Manager.Instance.ShowPopupUI<UI_CreditPopup>();
        }
        void OnClickXBtn(PointerEventData evt)
        {
            Debug.Log("X 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        #endregion

        #region 볼륨 설정
        void ChangeBGM(float value)
        {
            SoundManager.Instance.ChangeVolume(true, value);
        }
        void ChangeSFX(float value)
        {
            SoundManager.Instance.ChangeVolume(false, value);
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
