using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Client.SystemEnum;

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
            BindEvent(GetButton((int)Buttons.BTN_Continue).gameObject, OnClickContinueBtn);
            BindEvent(GetButton((int)Buttons.BTN_Renew).gameObject, OnClickRenewBtn);
            BindEvent(GetButton((int)Buttons.BTN_Title).gameObject, OnClickTitleBtn);
        }

        void BindSlider()
        {
            Get<Slider>((int)Sliders.Slider_BGM).onValueChanged.AddListener(ChangeBGM);
            Get<Slider>((int)Sliders.Slider_SFX).onValueChanged.AddListener(ChangeSFX);
            Get<Slider>((int)Sliders.Slider_BGM).value = LoadBGMVolume();
            Get<Slider>((int)Sliders.Slider_SFX).value = LoadSFXVolume();

        }

        void OnClickPanel(PointerEventData evt)
        {
            ClosePopupUI();
            SoundManager.Instance.Play(eSound.SFX_Negative);
        }
        void OnClickContinueBtn(PointerEventData evt)
        {
            Debug.Log("이어하기 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);
            ClosePopupUI();
        }
        /// <summary>
        /// 새로하기하면 데이터 초기화하고 타이틀로 돌아감
        /// </summary>
        /// <param name="evt"></param>
        void OnClickRenewBtn(PointerEventData evt)
        {
            Debug.Log("새로하기 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);

            DataManager.Instance.playerData = new PlayerData();

            // LoadTitleScreen();

            ClosePopupUI();
            SceneManager.LoadScene("TitleScene");
        }
        /// <summary>
        /// 바로 타이틀로 돌아감
        /// </summary>
        /// <param name="evt"></param>
        void OnClickTitleBtn(PointerEventData evt)
        {
            Debug.Log("타이틀로 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);

            // 타이틀로 돌아가기
            // LoadTitleScreen();

            ClosePopupUI();
            SceneManager.LoadScene("TitleScene");
        }

        /*
        void LoadTitleScreen(){
            ClosePopupUI();
            SceneManager.LoadScene("TitleScene");
        }
         */


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

    }
}
