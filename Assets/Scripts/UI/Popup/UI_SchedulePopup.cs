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
        GameObject[] scheduleList = new GameObject[7];
        int turn;
        int[] scheduledTurn = {0, 5, 11, 11, 17, 21, 22}; // 턴 0, 5, 11, 11, 17, 21, 22에 이벤트 있음? <- 학사일정UI 업데이트
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
            turn = GameManager.Data.playerData.currentTurn;
            for (int i = 0; i < 7; i++) scheduleList[i] = transform.GetChild(i+3).gameObject;

            SchedulePopupUpdate(turn);
        }
        /// <summary>
        /// 학사일정 켜질때 업데이트
        /// </summary>
        private void OnEnable()
        {
            turn = GameManager.Data.playerData.currentTurn;
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
            for (temp = 0; f_turn > scheduledTurn[temp]; temp++) ; // 턴 0, 5, 11, 11, 17, 21, 22에 이벤트 있음? <- 학사일정UI 업데이트
            for (int i = 0; i < temp; i++)
            {
                scheduleList[i].GetComponent<Image>().color = new Color(106/255f, 106/255f, 106/255f, 1f);
                scheduleList[i].transform.GetChild((int)Contents.IMG_Cancel).gameObject.SetActive(true);
            }
        }
    }
}