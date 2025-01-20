using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.Define;

namespace Client
{
    public class UI_UnlockedEndingPopup : UI_Popup
    {
        enum Buttons
        {
            Panel,
        }

        enum Texts
        {
            TMP_Title, TMP_Name, TMP_Contents,
        }

        enum Images
        {
            IMG_Illustration,
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<TMPro.TMP_Text>(typeof(Texts));
            Bind<Image>(typeof(Images));
            BindButton();
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
        }

        #region 버튼 이벤트
        void OnClickPanel(PointerEventData evt)
        {
            Debug.Log("판넬 누름..");
            ClosePopupUI();
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
                // GetText((int)Texts.TMP_Contents).text = ending.awards;
            }
        }
    }
}

