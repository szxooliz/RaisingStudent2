using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;
using static UnityEditor.Timeline.TimelinePlaybackControls;

namespace Client
{
    public class UI_LogPopup : UI_Popup
    {
        // 프로세스 하나 진행할 때마다 그 프로세스의 정보를 담은 로그 박스 프리팹 생성 및 캐시에 추가하는 함수 작성하고 호출하도록
        // 여기서는 캐시에 있는 프리팹을 로그에 띄워주기만 하기

        // TODO : 활동, 이벤트 담은 프리팹 불러오기 + 캐시 사용
        enum Buttons
        {
            Panel,
            BTN_Close
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            BindButton();
        }

        private void OnEnable()
        {
            // 진행한 프로세스 개수만큼 프리팹을 캐시에서 instanciate
        }



        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_Close).gameObject, OnClickCloseBtn);
        }

        void OnClickCloseBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        void OnClickPanel(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
    }

}
