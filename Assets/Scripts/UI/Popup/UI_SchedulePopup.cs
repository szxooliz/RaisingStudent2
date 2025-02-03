using Client;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    public class UI_SchedulePopup : UI_Popup
    {
        GameObject[] scheduleList = new GameObject[6]; // 스케줄 팝업 구성 요소 게임오브젝트 배열
        int turn; // 현재 몇턴인지 받아올 인자

        // 스케줄 업데이트에 사용하는 리소스 - 일단 하드코딩
        public static int[] scheduledTurn = {0, 5, 11, 11, 17, 23}; // 고정이벤트가 일어나는 턴
        public static string[] scheduleText = {"개강", "1학기 중간고사", "1학기 기말고사", "여름방학", "2학기 중간고사", "2학기 기말고사"}; // 고정이벤트 내용

        enum Buttons
        {
            Panel,
            BTN_Close
        }
        enum Contents
        {
            TXT_Month, TXT_Event,
            IMG_Cancel
        }
        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            BindButton();

            turn = DataManager.Instance.playerData.currentTurn; // 턴을 받아옴

            for (int i = 0; i < scheduledTurn.Length; i++)
            {
                // scheduleList에 게임 오브젝트 받아오기
                scheduleList[i] = transform.GetChild(i + 3).gameObject;
                // 스케줄 팝업창 요소별 텍스트를 scheduleText로 넣어줌
                scheduleList[i].transform.GetChild((int)Contents.TXT_Event).GetComponent<TMP_Text>().text = scheduleText[i]; 
            }

            SchedulePopupUpdate(turn);
        }
        /// <summary>
        /// 학사일정 켜질때 업데이트
        /// </summary>
        private void OnEnable()
        {
            turn = DataManager.Instance.playerData.currentTurn;
            SchedulePopupUpdate(turn);
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_Close).gameObject, OnClickCloseBtn);
        }
        void OnClickPanel(PointerEventData evt)
        {
            ClosePopupUI();
        }

        void OnClickCloseBtn(PointerEventData evt)
        {
            ClosePopupUI();
        }
        /// <summary>
        /// 학사일정 지나가면 취소표 그려주는 함수
        /// </summary>
        void SchedulePopupUpdate(int f_turn)
        {
            int temp;
            for (temp = 0; f_turn > scheduledTurn[temp]; temp++) ;
            for (int i = 0; i < temp; i++)
            {
                scheduleList[i].GetComponent<Image>().color = new Color(106/255f, 106/255f, 106/255f, 1f);
                scheduleList[i].transform.GetChild((int)Contents.IMG_Cancel).gameObject.SetActive(true);
            }
        }
    }
}