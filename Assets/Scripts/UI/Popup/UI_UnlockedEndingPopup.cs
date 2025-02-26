using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class UI_UnlockedEndingPopup : UI_Popup
    {
        enum Buttons
        {
            Panel, BTN_X,
        }

        enum Texts
        {
            TMP_Title, TMP_Name, TMP_Awards, TMP_Contents,
        }

        enum Images
        {
            IMG_Illustraion,
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<TMPro.TMP_Text>(typeof(Texts));
            Bind<Image>(typeof(Images));
            BindButton();
            GetImage((int)Images.IMG_Illustraion).gameObject.AddComponent<Button>().onClick.AddListener(OnClickImage);
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_X).gameObject, OnClickXBtn);
        }

        #region 버튼 이벤트
        void OnClickPanel(PointerEventData evt)
        {
            Debug.Log("판넬 누름..");
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        void OnClickXBtn(PointerEventData evt)
        {
            Debug.Log("x 버튼 누름");
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        void OnClickImage()
        {
            Debug.Log("이미지 누름");
            SoundManager.Instance.Play(eSound.SFX_DialogClick);
        }
        #endregion

        /// <summary>
        /// UnlockedEndingPopup 데이터 설정 함수
        /// </summary>
        /// <param name="ending"></param>
        public void SetUnlockedEndingPopup(Ending ending)
        {
            if (ending != null) {
                GetText((int)Texts.TMP_Title).text = GetEndingNameKor(ending.endingName) + " - 열람 중";
                GetText((int)Texts.TMP_Name).text = GetEndingNameKor(ending.endingName);
                GetText((int)Texts.TMP_Awards).text = string.Join(" ", ending.awards);
            }
        }
    }
}

