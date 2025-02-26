using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class UI_LockedEndingPopup : UI_Popup
    {
        private string spritePath = "Sprites/UI/Ending/";

        enum Buttons
        {
            Panel,
            BTN_Retry, BTN_Cancel,
        }

        enum Images
        {
            IMG_File,
        }

        enum Texts
        {
            TMP_Name,
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<Image>(typeof(Images));
            Bind<TMPro.TMP_Text>(typeof(Texts));
            BindButton();
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_Retry).gameObject, OnClickRetryBtn);
            BindEvent(GetButton((int)Buttons.BTN_Cancel).gameObject, OnClickCancelBtn);
        }

        #region 버튼 이벤트
        void OnClickPanel(PointerEventData evt)
        {
            Debug.Log("판넬 누름..");
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        void OnClickRetryBtn(PointerEventData evt)
        {
            Debug.Log("다시 시도 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);
        }
        void OnClickCancelBtn(PointerEventData evt)
        {
            Debug.Log("취소 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        #endregion

        /// <summary>
        /// LockedEndingPopup 데이터 설정 함
        /// </summary>
        /// <param name="ending"></param>
        public void SetLockedEndingPopup(Ending ending)
        {
            if (ending != null)
            {
                int index = (int)(ending.endingName);
                GetText((int)Texts.TMP_Name).text = "파일명: 엔딩" + (char)('A' + index);
                GetImage((int)Images.IMG_File).sprite = DataManager.Instance.GetOrLoadSprite(spritePath + (char)('A' + index) + "_lock");
            }
        }
    }
}
