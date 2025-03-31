using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class UI_CreditPopup : UI_Popup
    {
        enum Buttons
        {
            Panel,
            BTN_X,
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
            BindEvent(GetButton((int)Buttons.BTN_X).gameObject, OnClickXBtn);
        }

        void OnClickPanel(PointerEventData evt)
        {
            Debug.Log("판넬 누름..");
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }

        void OnClickXBtn(PointerEventData evt)
        {
            Debug.Log("X 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
    }
}
