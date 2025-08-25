using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class UI_BaseScene : UI_Scene
    {
        enum Buttons
        {
            BTN_Menu, BTN_Schedule, BTN_Log
        }
        enum Texts
        {
            TMP_Turn, TMP_Term
        }
        enum Images
        {
            IMG_Status, //IMG_Black
        }

        private string spritePath = "Sprites/UI/Status/Status_";
        private Image blackImage;
        private float duration = 0.5f;
        private bool isLogButtonInteractable = true;
        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<TMPro.TMP_Text>(typeof(Texts));
            Bind<Image>(typeof(Images));

            BindButton();

            GameManager.Instance.OnActivity += SetButtonOnOff;
            GameManager.Instance.OnSelection += SetScheduleOnOff;
            DataManager.Instance.playerData.OnStatusChanged += OnStatusChanged;
            EventManager.Instance.OnEventStart += ShowEventName;
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.BTN_Menu).gameObject, OnClickMenuBtn);
            BindEvent(GetButton((int)Buttons.BTN_Schedule).gameObject, OnClickScheduleBtn);
            BindEvent(GetButton((int)Buttons.BTN_Log).gameObject, OnClickLogBtn);
        }

        private void Start()
        {
            UpdateTermUI();
            UpdateTurnUI();
        }

        private void OnDestroy()
        {
            DataManager.Instance.playerData.OnStatusChanged -= OnStatusChanged;
            GameManager.Instance.OnActivity -= SetButtonOnOff;
            GameManager.Instance.OnSelection -= SetScheduleOnOff;
            EventManager.Instance.OnEventStart -= ShowEventName;
        }

        #region 버튼 이벤트
        void OnClickMenuBtn(PointerEventData evt)
        {
            Debug.Log("메뉴 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);
            UI_Manager.Instance.ShowPopupUI<UI_MenuPopup>();
        }

        void OnClickScheduleBtn(PointerEventData evt)
        {
            Debug.Log("학사 일정 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);
            UI_Manager.Instance.ShowPopupUI<UI_SchedulePopup>();
        }

        void OnClickLogBtn(PointerEventData evt)
        {
            //Debug.Log("로그 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);
            UI_Manager.Instance.ShowPopupUI<UI_LogPopup_Simple>();
        }

        #endregion

        void OnStatusChanged(object sender, System.EventArgs e)
        {
            if (this == null) return; // 오브젝트 자체가 사라진 경우
            var img = GetImage((int)Images.IMG_Status);
            if (img == null) return; // 이미지가 이미 Destroy된 경우

            string path = "";

            switch (DataManager.Instance.playerData.CurrentStatus)
            {
                case eStatus.Main:
                    GameManager.Instance.NextMonthandTerm();
                    UpdateTermUI();
                    UpdateTurnUI();
                    path = spritePath + eStatus.Main.ToString();
                    break;
                case eStatus.Activity:
                    path = spritePath + eStatus.Activity.ToString();
                    break;
                case eStatus.Event:
                    path = spritePath + eStatus.Event.ToString();
                    break;
            }

            img.sprite = DataManager.Instance.GetOrLoadSprite(path);
        }

        // TODO : 상황에 따른 로그 버튼 활성/비활성 함수 만들기 - 반투명 이미지를 위에 붙여서 활성화
        // 활동 때, 이벤트 마지막에 스탯 변경될 때 비활성화
        void SetButtonOnOff()
        {
            isLogButtonInteractable = !isLogButtonInteractable;
            // isLogButtonInteractable값과 반대로 버튼 활성화 설정, 그리고 그 반대값 넣기
            GetButton((int)Buttons.BTN_Log).interactable = !isLogButtonInteractable;
            GetButton((int)Buttons.BTN_Schedule).interactable = !isLogButtonInteractable;
        }
        void SetScheduleOnOff()
        {
            isLogButtonInteractable = !isLogButtonInteractable;
            // isLogButtonInteractable값과 반대로 버튼 활성화 설정, 그리고 그 반대값 넣기
            GetButton((int)Buttons.BTN_Schedule).interactable = !isLogButtonInteractable;
        }

        /// 학사 일정 표시된 주요 이벤트까지 남은 턴 표시
        /// </summary>
        void UpdateTurnUI()
        {
            int turn = DataManager.Instance.playerData.CurrentTurn;
            string str = "";

            long index = (int)EventManager.Instance.GetNextScheduleID();

            if (index == -1) return;
            else
            {
                EventTitle eventTitle = DataManager.Instance.GetData<EventTitle>(index);

                int nextTurn = (int)eventTitle.AppearStart;
                str = $"앞으로 {nextTurn - turn}턴";
            }
            GetText((int)Texts.TMP_Turn).text = str;
        }

        /// <summary>
        /// 시기 표시 UI 업데이트
        /// </summary>
        void UpdateTermUI()
        {
            string str = (int)DataManager.Instance.playerData.CurrentMonth + "월 " + GetThirdsKor(DataManager.Instance.playerData.CurrentThird);

            GetText((int)Texts.TMP_Term).text = str;
            Debug.Log($"UI 턴 : {str}, 숫자로: {DataManager.Instance.playerData.CurrentTurn}");
        }

        /// <summary>
        /// 이벤트 실행 시에 턴 대신 이벤트 이름 표시 함수
        /// </summary>
        public void ShowEventName()
        {
            GetText((int)Texts.TMP_Turn).text = EventManager.Instance.nowEventData.eventTitle.EventName;
        }


    }
}
