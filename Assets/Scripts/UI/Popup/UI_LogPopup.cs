using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    public class UI_LogPopup : UI_Popup
    {
        // TODO : 활동, 이벤트 담은 프리팹 불러오기 + 캐시 사용

        enum Buttons
        {
            BTN_Close
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            BindButton();
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.BTN_Close).gameObject, OnClickCloseBtn);
        }

        void OnClickCloseBtn(PointerEventData evt)
        {
            ClosePopupUI();
        }
    }

}
