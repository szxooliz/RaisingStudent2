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
        GameObject[] gameObjects = new GameObject[7];
        int turn, counter;
        int[] ScheduledTurn = {0, 5, 11, 11, 17, 21, 22}; // 턴 0, 5, 11, 11, 17, 21, 22에 이벤트 있음? <- 학사일정UI 업데이트
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
            for (int i = 0; i < 7; i++) gameObjects[i] = transform.GetChild(i+3).gameObject;

            for (counter = 0; turn >= ScheduledTurn[counter]; counter++) ; // 턴 0, 5, 11, 11, 17, 21, 22에 이벤트 있음? <- 학사일정UI 업데이트
            SchedulePopupUpdate(counter);
        }
        /// <summary>
        /// 학사일정 켜질때 업데이트
        /// </summary>
        private void OnEnable()
        {
            turn = GameManager.Data.playerData.currentTurn;
            for (counter = 0; turn > ScheduledTurn[counter]; counter++) ;
            SchedulePopupUpdate(counter);
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
        public void SchedulePopupUpdate(int temp)
        {
            for (int i = 0; i < temp; i++)
            {
                gameObjects[i].GetComponent<Image>().color = new Color(106/255f, 106/255f, 106/255f, 1f);
                gameObjects[i].transform.GetChild((int)Contents.IMG_Cancel).gameObject.SetActive(true);
            }
        }
    }
}