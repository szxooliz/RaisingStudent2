using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Client
{
    public class UI_BaseScene : UI_Scene
    {
        /*
        필요한 것 정리
        1. 스탯 이름, 스탯 수치
        2. 스트레스 수치
        3. 활동 버튼들 -> 누르면 활동 이벤트 발생
        4. 턴 기록
        5. 스케줄 버튼 -> 팝업 O
        6. 로그 버튼 -> 팝업 O
        7. 현재 상황 표시: 로그버튼 옆에있던거
        8. 메뉴 버튼 -> 팝업 O
        */

        enum Buttons
        {
            BTN_Menu, BTN_Schedule,
            BTN_Log,
            BTN_Rest,
            BTN_Study, BTN_Game, BTN_WorkOut, BTN_Club,
            // MaxCount
        }

        enum Texts
        {
            // 스탯 이름들
            TMP_InteliName, TMP_OtakuName, TMP_StrengthName, TMP_CharmingName,
            
            // 스탯 수치들
            TMP_Inteli, TMP_Otaku, TMP_Strength, TMP_Charming,

            //MaxCount
        }

        // TODO: UI_Stress Slider Binding

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<TMPro.TMP_Text>(typeof(Texts));
            BindButton();
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.BTN_Menu).gameObject, OnClickMenuBtn);
            BindEvent(GetButton((int)Buttons.BTN_Schedule).gameObject, OnClickScheduleBtn);
            BindEvent(GetButton((int)Buttons.BTN_Log).gameObject, OnClickLogBtn);

            BindEvent(GetButton((int)Buttons.BTN_Rest).gameObject, OnClickRestBtn);
            BindEvent(GetButton((int)Buttons.BTN_Study).gameObject, OnClickStudyBtn);
            BindEvent(GetButton((int)Buttons.BTN_Game).gameObject, OnClickGameBtn);
            BindEvent(GetButton((int)Buttons.BTN_WorkOut).gameObject, OnClickWorkOutBtn);
            BindEvent(GetButton((int)Buttons.BTN_Club).gameObject, OnClickClubBtn);

        }

        #region 버튼 이벤트
        void OnClickMenuBtn(PointerEventData evt)
        {
            Debug.Log("메뉴 버튼 클릭");
        }

        void OnClickScheduleBtn(PointerEventData evt)
        {
            Debug.Log("학사 일정 버튼 클릭");
        }

        void OnClickLogBtn(PointerEventData evt)
        {
            Debug.Log("로그 버튼 클릭");
            // UI_Manager.Instance.ShowPopupUI<UI_LogPopup>();
            // 갑자기 안되네..
        }

        void OnClickRestBtn(PointerEventData evt)
        {
            Debug.Log("자체 휴강 버튼 클릭");
        }
        void OnClickStudyBtn(PointerEventData evt) 
        {
            Debug.Log("공부 버튼 클릭");
        }
        void OnClickGameBtn(PointerEventData evt) 
        {
            Debug.Log("게임 버튼 클릭");
        }
        void OnClickWorkOutBtn(PointerEventData evt) 
        {
            Debug.Log("운동 버튼 클릭");
        }
        void OnClickClubBtn(PointerEventData evt) 
        {
            Debug.Log("동아리 버튼 클릭");
        }
        #endregion

    }

}
