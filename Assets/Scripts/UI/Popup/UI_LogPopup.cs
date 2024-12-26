using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    public class UI_LogPopup : UI_Popup
    {
        /*
         기본: 캐릭터 대사 띄우기
         인터랙션: 기존 말풍선 축소 후 확대 애니메이션(dotween) + 텍스트 리뉴얼
         */

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
