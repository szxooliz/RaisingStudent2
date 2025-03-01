using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Client.SystemEnum;
using System.Linq;
using UnityEditor.PackageManager;

namespace Client
{
    public class EventManager : Singleton<EventManager>
    {
        #region constant - 기획 조정용
        private const int MAIN_COUNT = 10; // 메인 이벤트 개수

        private const int RANDOM_THRESHOLD = 100;         // 랜덤 이벤트 ID 시작 인덱스
        private const int RANDOM_SCRIPT_THRESHOLD = 1000; // 랜덤 이벤트 스크립트 시작 인덱스
        private const int RANDOM_COUNT = 26;              // 랜덤 이벤트 개수
        private const int RANDOM_PROB = 50;               // 랜덤 이벤트 등장 확률
        #endregion

        public Queue<(long ID, string title)> EventIDQueue { get; } = new();
        public Queue<EventData> EventQueue { get; } = new();
        public Dictionary<long, EventResult> EventResults { get; } = new(); // key : ScriptIndex

        public EventData nowEventData;
        public Action OnEventStart;

        #region Singleton
        EventManager() { }
        #endregion

        /// <summary>
        /// 등장 조건에 맞춰 실행해야 하는 이벤트 인덱스 가져오기
        /// </summary>
        private void EnqueueEventID()
        {
            // 메인 이벤트 가져오기
            for (int i = 0; i <= MAIN_COUNT; i++)
            {
                EventTitle eventTitle = DataManager.Instance.GetData<EventTitle>(i);

                //// 해커톤 참여 선택에 따라 추가
                //if (!DataManager.Instance.playerData.IsEnrolled) continue;

                if (DataManager.Instance.playerData.CurrentTurn == eventTitle.AppearStart)
                {
                    EventIDQueue.Enqueue((eventTitle.index, eventTitle.EventName));
                }
            }

            List<EventTitle> randomList = new List<EventTitle>();

            // 랜덤 이벤트 가져오기
            for (int i = RANDOM_THRESHOLD; i <= RANDOM_THRESHOLD + RANDOM_COUNT; i++)
            {
                EventTitle eventTitle = DataManager.Instance.GetData<EventTitle>(i);

                if (IsInTurnRange(eventTitle) && TriggerRandomEvent())
                {
                    randomList.Add(eventTitle);
                }
            }

            // 랜덤 이벤트 한 개만 추가
            EventTitle randomEventTitle = GetOneRandomEvent(randomList);
            if (randomEventTitle != null)
            {
                EventIDQueue.Enqueue((randomEventTitle.index, randomEventTitle.EventName));
            }
                
        }

        /// <summary>
        /// 한 턴에 랜덤 이벤트 최대 1개, 재등장 불가
        /// </summary>
        /// <param name="randomList"></param>
        /// <returns></returns>
        private EventTitle GetOneRandomEvent(List<EventTitle> randomList)
        {
            try
            {
                DeleteWatchedEvent(randomList);
                return randomList[UnityEngine.Random.Range(0, randomList.Count)];
            }
            catch
            {
                return null;
            }
        }

        #region 이벤트 진행 상황
        /// <summary>
        /// 봤던 이벤트로 기록
        /// </summary>
        /// <param name="eventData"></param>
        public void AddWatchedEvent(EventData eventData)
        {
            try
            {
                Debug.Log($"이벤트 끝, {eventData.title} 등록");
                DataManager.Instance.playerData.WatchedEvents.Add(eventData.eventIndex, eventData);
            }
            catch
            {
                Debug.LogError($"{eventData.title} : {eventData.eventIndex}는 이미 등록된 이벤트 인덱스입니다");
                return;
            }
        }

        /// <summary>
        /// 후보 랜덤 이벤트 중에서 이미 봤던 이벤트는 제외
        /// </summary>
        /// <param name="randomList"></param>
        private void DeleteWatchedEvent(List<EventTitle> randomList)
        {
            // 역 for문 사용
            for (int i = randomList.Count - 1; i >= 0; i--)
            {
                if (DataManager.Instance.playerData.WatchedEvents.ContainsKey(randomList[i].index))
                {
                    randomList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 학사 일정 이벤트 중에서 watchedEvent에 기록된 게 있으면 제일 마지막 기록된 인덱스 리턴
        /// </summary>
        /// <returns></returns>
        public long GetLargestScheduleID()
        {
            List<eScheduleEvent> scheduleList = new List<eScheduleEvent>((eScheduleEvent[])Enum.GetValues(typeof(eScheduleEvent)));
            long watchedEventID = -1; 

            // 역순환 -  기록된 것 중에 가장 큰 스케줄 인덱스를 가져오기 위해
            scheduleList.Reverse();
            foreach(eScheduleEvent sch in scheduleList)
            {
                if(DataManager.Instance.playerData.WatchedEvents.ContainsKey((int)sch))
                {
                    watchedEventID = (long)sch;
                    break;
                }
            }
            return watchedEventID;
        }

        /// <summary>
        /// 다음 실행될 예정인 학사일정 이벤트 enum 리턴
        /// </summary>
        /// <returns></returns>
        public long GetNextScheduleID()
        {
            List<eScheduleEvent> scheduleList = new List<eScheduleEvent>((eScheduleEvent[])Enum.GetValues(typeof(eScheduleEvent)));
            int schIndex = -1;

            if (DataManager.Instance.playerData.WatchedEvents.Count == 0)
                return -1;

            // 역순환 -  기록된 것 중에 가장 큰 스케줄ID를 가져오기 위해
            scheduleList.Reverse();
            foreach (eScheduleEvent sch in scheduleList)
            {
                if (DataManager.Instance.playerData.WatchedEvents.ContainsKey((int)sch))
                {
                    schIndex = scheduleList.IndexOf(sch);
                    break;
                }
            }

            eScheduleEvent scheduleEvent = scheduleList[schIndex - 1];
            return (long)scheduleEvent;
        }
        #endregion

        /// <summary>
        /// 등장 가능 범위 내의 이벤트인지 확인
        /// </summary>
        /// <param name="eventTitle"></param>
        /// <returns></returns>
        private bool IsInTurnRange(EventTitle eventTitle)
        {
            int currentTurn = DataManager.Instance.playerData.CurrentTurn;
            return currentTurn >= eventTitle.AppearStart && currentTurn <= eventTitle.AppearEnd;
        }

        /// <summary>
        /// 랜덤 이벤트 자체 발생 확률 계산
        /// </summary>
        /// <returns></returns>
        private bool TriggerRandomEvent()
        {
            return UnityEngine.Random.Range(0, 101) <= RANDOM_PROB;
        }

        /// <summary>
        /// 이벤트 데이터를 로드해서 대기열에 추가
        /// </summary>
        private void LoadEventData()
        {
            if (EventIDQueue.Count == 0) return;

            while (EventIDQueue.Count > 0)
            {
                var (eventID, eventTitle) = EventIDQueue.Dequeue();
                eEventType eventType = (eventID > RANDOM_THRESHOLD) ? eEventType.Random : eEventType.Main;
                Dictionary<long, EventScript> eventScripts = LoadScript(eventID, eventType);

                EventData eventData = new EventData(eventID, eventType, eventScripts) { title = eventTitle };
                Debug.Log($"EventQueue에 추가된 이벤트: {eventData.title}");
                EventQueue.Enqueue(eventData);
            }
        }

        /// <summary>
        /// 외부에서 이벤트 확인하고 로드할 때 호출
        /// </summary>
        public void CheckEvent()
        {
            EnqueueEventID();
            LoadEventData();

            if (EventQueue.Count > 0) StartEventPhase();
        }

        /// <summary>
        /// 이벤트 페이즈로 넘어감
        /// </summary>
        private void StartEventPhase()
        {
            DataManager.Instance.playerData.CurrentStatus = eStatus.Event;
        }

        /// <summary>
        /// 이벤트 인덱스에 맞는 이벤트 스크립트 전체 불러오기
        /// </summary>
        private Dictionary<long, EventScript> LoadScript(long eventID, eEventType eventType)
        {
            Dictionary<long, EventScript> eventScripts = new Dictionary<long, EventScript>();
            int tempIndex = eventType == eEventType.Random ? RANDOM_SCRIPT_THRESHOLD : 0;

            while (true)
            {
                try
                {
                    EventScript eventScript = DataManager.Instance.GetData<EventScript>(tempIndex);

                    if (eventScript.EventNum == eventID)
                    {
                        eventScripts.Add(eventScript.index, eventScript);
                    }
                }
                catch
                {
                    break;
                }

                tempIndex++;
            }

            return eventScripts;
        }
    }
}