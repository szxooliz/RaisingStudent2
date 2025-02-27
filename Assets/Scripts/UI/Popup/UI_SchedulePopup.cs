using Client;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using static Client.SystemEnum;
using System;

namespace Client
{
    public class UI_SchedulePopup : UI_Popup
    {
        // key: 이벤트 인덱스, value: 진행 여부
        Dictionary<int, bool> scheduleEvent = new Dictionary<int, bool>();

        // 학사일정 컨텐츠 박스들
        Dictionary<int, ScheduleContent> scheduleContentMap = new Dictionary<int, ScheduleContent>();
        [SerializeField] List<ScheduleContent> scheduleContents = new List<ScheduleContent>();

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
            InitSchedule();
        }

        private void OnEnable()
        {
            UpdateSchedule();
            UpdateScheduleUI();
        }

        #region Button
        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_Close).gameObject, OnClickCloseBtn);
        }

        void OnClickPanel(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }

        void OnClickCloseBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        #endregion

        /// <summary>
        /// 스케줄 기록 초기화
        /// </summary>
        public void InitSchedule()
        {
            List<eScheduleEvent> scheduleList = new List<eScheduleEvent>((eScheduleEvent[])Enum.GetValues(typeof(eScheduleEvent)));

            // 이벤트 진행여부 딕셔너리 초기화
            scheduleEvent.Clear();
            foreach (eScheduleEvent eSchedule in scheduleList)
            {
                // 진행한 이벤트 중 스케줄이 있으면 true, 없으면 false
                if (DataManager.Instance.playerData.watchedEvents.ContainsKey((int)eSchedule))
                {
                    Debug.Log($"watchedEvents에 Key인 {eSchedule.ToString()} 추가");
                    scheduleEvent.Add((int)eSchedule, true);
                }
                else
                {
                    scheduleEvent.Add((int)eSchedule, false);
                }
            }

            // 스케줄 컨텐츠 박스 UI 리스트로 가져오기
            scheduleContents.Clear();
            scheduleContents = new List<ScheduleContent>(GetComponentsInChildren<ScheduleContent>(true));

            // 스케줄 컨텐츠 박스 UI 상태 초기화
            scheduleContentMap.Clear();

            if (scheduleContents.Count != scheduleList.Count)
            {
                Debug.LogError($"⚠️ scheduleContents {scheduleContents.Count} != scheduleList {scheduleList.Count}");
                return;
            }

            for (int i = 0; i < scheduleContents.Count; i++)
            {
                int eventIndex = (int)scheduleList[i];                // Enum의 int 값 가져오기
                scheduleContents[i].Initialize(eventIndex);           // ScheduleContent에 Index 설정
                scheduleContentMap[eventIndex] = scheduleContents[i]; // Dictionary에 저장
            }
        }

        public void UpdateSchedule()
        {
            // 지나간 이벤트, 진행중인 이벤트를 딕셔너리에 기록하도록            
            // nowEventData.eventIndex를 기준으로, 이 값보다 작은 key 값의 value는 true로 설정
            // 값이 같은 경우에는 동그라미를 쳐야 하니까..

            int nowEvtIndex = (int)EventManager.Instance.GetLargestScheduleID();
            Debug.Log($"가장 마지막에 본 학사일정 아이디 : {nowEvtIndex}");
            foreach (int key in new List<int>(scheduleEvent.Keys))
            {
                scheduleEvent[key] = key <= nowEvtIndex;
            }
        }

        /// <summary>
        /// 현재 이벤트 진행사항 UI 표시 적용 함수
        /// </summary>
        public void UpdateScheduleUI()
        {
            // 취소선의 경우 딕셔너리 value == true 인 경우에 활성화
            // 동그라미의 경우 이벤트매니저의 nowEventData.eventIndex의 값이 딕셔너리의 키 값이 같으면 활성화 

            int nowEvtIndex = (int)EventManager.Instance.nowEventData.eventIndex;

            foreach (var kvp in scheduleEvent)
            {
                if (scheduleContentMap.TryGetValue(kvp.Key, out var content))
                {
                    content.ToggleLine(kvp.Value);
                    content.ToggleCircle(kvp.Key == nowEvtIndex);
                }
            }
        }

    }
}