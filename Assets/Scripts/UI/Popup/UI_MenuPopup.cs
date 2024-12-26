using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    public class UI_MenuPopup : UI_Popup
    {
        private const string BGMvolKey = "BGMvol";
        private const string SFXvolKey = "SFXvol";

        enum Buttons
        {
            Panel,
            BTN_Continue, BTN_Renew, BTN_Title
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

            //BindSlider();
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_Continue).gameObject, OnClickContinueBtn);
            BindEvent(GetButton((int)Buttons.BTN_Renew).gameObject, OnClickRenewBtn);
            BindEvent(GetButton((int)Buttons.BTN_Title).gameObject, OnClickTitleBtn);
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

        void OnClickContinueBtn(PointerEventData evt)
        {
            Debug.Log("이어하기 버튼 클릭");
        }
        void OnClickRenewBtn(PointerEventData evt)
        {
            Debug.Log("새로하기 버튼 클릭");
        }

        void OnClickTitleBtn(PointerEventData evt)
        {
            Debug.Log("타이틀로 버튼 클릭");
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
