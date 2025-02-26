using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class UI_TitleScene : UI_Scene
    {
        enum Buttons
        {
            BTN_NewGame, BTN_Continue, BTN_EndingList, BTN_Settings,
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));

            BindButton();
        }

        /// <summary>
        /// 버튼 바인딩
        /// </summary>
        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.BTN_NewGame).gameObject, OnClickNewGameBtn);
            BindEvent(GetButton((int)Buttons.BTN_Continue).gameObject, OnClickContinueBtn);
            BindEvent(GetButton((int)Buttons.BTN_EndingList).gameObject, OnClickEndingListBtn);
            BindEvent(GetButton((int)Buttons.BTN_Settings).gameObject, OnClickSettingsBtn);
        }

        #region Title 버튼 이벤트
        void OnClickNewGameBtn(PointerEventData evt)
        {
            Debug.Log("새로 하기 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);
            UI_Manager.Instance.ShowPopupUI<UI_NewGamePopup>();
        }
        void OnClickContinueBtn(PointerEventData evt)
        {
            Debug.Log("이어 하기 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);
        }
        void OnClickEndingListBtn(PointerEventData evt)
        {
            Debug.Log("엔딩 리스트 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);
        }
        void OnClickSettingsBtn(PointerEventData evt)
        {
            Debug.Log("설정 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);
            UI_Manager.Instance.ShowPopupUI<UI_SettingsPopup>();
        }
        #endregion
    }
}
