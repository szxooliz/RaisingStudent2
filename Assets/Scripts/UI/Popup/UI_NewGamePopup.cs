using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class UI_NewGamePopup : UI_Popup
    {
        enum Buttons
        {
            Panel,
            BTN_Yes, BTN_No,
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            BindButton();
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_Yes).gameObject, OnClickYesBtn);
            BindEvent(GetButton((int)Buttons.BTN_No).gameObject, OnClickNoBtn);
        }

        #region 버튼 이벤트
        void OnClickPanel(PointerEventData evt)
        {
            Debug.Log("판넬 누름..");
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        void OnClickYesBtn(PointerEventData evt)
        {
            Debug.Log("예 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);

            // 플레이어 데이터를 초기화 시킨 후 게임 화면으로 이동 
            DataManager.Instance.playerData = new PlayerData();

            ClosePopupUI();
            SceneManager.LoadScene("BaseScene");
        }
        void OnClickNoBtn(PointerEventData evt)
        {
            Debug.Log("아니오 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        #endregion
    }
}
