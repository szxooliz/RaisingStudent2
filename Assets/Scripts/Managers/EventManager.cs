using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Client.SystemEnum;
using Unity.VisualScripting;

namespace Client
{
    public class EventManager : Singleton<EventManager>
    {
        #region constant - 기획 조정용
        private const int RANDOM_THRESHOLD = 100;         // 랜덤 이벤트 ID 시작 인덱스
        private const int RANDOM_PROB = 50;               // 랜덤 이벤트 등장 확률
        #endregion

        List<SheetData> eventTitleList;

        public Queue<EventTitle> EventIDQueue { get; } = new();
        public Queue<EventData> EventQueue { get; } = new();

        public bool IsEventAllFinished = false;
        public EventTitle ApplingEvent; // 참가 여부 결정하는 이벤트 
        public bool IsEventApplied = false; // 이벤트 참가 여부 저장, 턴 종료 후 적용 
        public EventData nowEventData;
        public Action OnEventStart;

        #region Singleton
        EventManager() { }
        #endregion

        public override void Init()
        {
            EventIDQueue.Clear();
            EventQueue.Clear();

            eventTitleList = DataManager.Instance.GetDataList<EventTitle>();
        }

        public void ResetEventData()
        {
            EventIDQueue.Clear();
            EventQueue.Clear();
            SaveTempApplyEvent(null, false);
        }

        /// <summary> 등장 조건에 맞춰 실행해야 하는 이벤트 인덱스 가져오기 </summary>
        private void EnqueueEventID()
        {
            Debug.Log($"현재 턴 {DataManager.Instance.playerData.CurrentTurn}");
            List<EventTitle> mainList = new();
            List<EventTitle> randomList = new();

            // 랜덤 이벤트 발생 확률 체크 후 추가 작업 진행
            bool doRand = TriggerRandomEvent();

            foreach (var _eventTitle in eventTitleList)
            {
                var eventTitle = _eventTitle as EventTitle;

                // 1. 메인 이벤트 먼저 추가
                if (eventTitle.index < RANDOM_THRESHOLD)
                {
                    if (IsMainEventAddable(eventTitle))
                    {
                        mainList.Add(eventTitle);
                        Debug.Log($"추가한 이벤트 아이디 : {eventTitle.index} {eventTitle.EventName}");
                    }
                }

                // 2. 랜덤 이벤트 추가
                else
                {
                    if (DataManager.Instance.playerData.CurrentTurn == 0)
                    {
                        Debug.Log($"<color=red>인트로 때는 랜덤 이벤트 추가 안함</color>");
                        break;
                    }

                    // 랜덤 이벤트 발생 체크 후 추가 작업 진행
                    if (!doRand)
                    {
                        Debug.Log($"<color=red>확률에 따라 랜덤 이벤트 추가 안함</color>");
                        break;
                    }

                    // 발생 가능한 턴 범위에 있으면 후보 리스트에 추가
                    if (IsInTurnRange(eventTitle)) randomList.Add(eventTitle);
                }
            }

            // 2-1. 랜덤 이벤트 한 개만 추가
            EventTitle randomEventTitle = GetOneRandomEvent(randomList);
            if (randomEventTitle != null)
            {
                EventIDQueue.Enqueue(randomEventTitle);
                Debug.Log($"{randomEventTitle.EventName} 랜덤 이벤트 최종 추가");
            }

            // 3. 앞에서 정해둔 메인 이벤트 차례 맞춰 추가
            foreach(var _eventTitle in mainList)
            {
                EventIDQueue.Enqueue(_eventTitle);
            }
        }

        #region 이벤트 진행 상황, 조건 체크
        /// <summary> 메인 이벤트 등장 조건 체크 </summary>
        private bool IsMainEventAddable(EventTitle eventTitle)
        {
            if (DataManager.Instance.playerData.AppliedEventsDict.TryGetValue(eventTitle.index, out bool value))
            {
                // 이벤트에 참여하지 않는 것으로 기록되어 있다면 Queue에 추가하지 않는다
                Debug.Log($"이전에 {eventTitle.index}번 이벤트 참가 신청을 {value}로 함");
                if (!value) return false;
            }

            if (DataManager.Instance.playerData.WatchedEventIDList.Contains(eventTitle.index)) return false;

            if (DataManager.Instance.playerData.CurrentTurn == eventTitle.AppearStart) return true;
            else return false;
        }

        /// <summary> 봤던 이벤트로 기록 </summary>
        public void AddWatchedEvent(long eventID)
        {
            if (DataManager.Instance.playerData.WatchedEventIDList.Contains(eventID))
            {
                Debug.LogError($"{eventID}는 이미 등록된 이벤트 인덱스입니다");
                return;
            }

            Debug.Log($"이벤트 끝, {eventID} 등록");
            DataManager.Instance.playerData.WatchedEventIDList.Add(eventID);
            DataManager.Instance.SaveAllData();
        }

        /// <summary> 후보 랜덤 이벤트 중에서 이미 봤던 이벤트는 제외 </summary>
        private void DeleteWatchedEvent(List<EventTitle> randomList)
        {
            // 제거 때문에 역 for문 사용
            for (int i = randomList.Count - 1; i >= 0; i--)
            {
                if (DataManager.Instance.playerData.WatchedEventIDList.Contains(randomList[i].index))
                {
                    randomList.RemoveAt(i);
                }
            }
        }

        /// <summary> 한 턴에 랜덤 이벤트 최대 1개, 재등장 불가 </summary>
        private EventTitle GetOneRandomEvent(List<EventTitle> randomList)
        {
            if (randomList.Count == 0) return null;

            DeleteWatchedEvent(randomList);
            return randomList[UnityEngine.Random.Range(0, randomList.Count)];
        }

        /// <summary> 등장 가능 범위 내의 이벤트인지 확인 </summary>
        private bool IsInTurnRange(EventTitle eventTitle)
        {
            int currentTurn = DataManager.Instance.playerData.CurrentTurn;
            return currentTurn >= eventTitle.AppearStart && currentTurn <= eventTitle.AppearEnd;
        }

        /// <summary> 랜덤 이벤트 자체 발생 확률 계산 </summary>
        private bool TriggerRandomEvent()
        {
            bool tm = UnityEngine.Random.Range(0, 101) <= RANDOM_PROB;
            Debug.Log($"<color=blue>랜덤이벤트 발생 여부 {tm}</color>");
            return tm;
        }
        #endregion

        #region 학사일정 팝업 전달용
        /// <summary> 학사 일정 이벤트 중에서 watchedEvent에 기록된 게 있으면 제일 마지막 기록된 인덱스 리턴 </summary>
        public long GetLargestScheduleID()
        {
            List<eScheduleEvent> scheduleList = new List<eScheduleEvent>((eScheduleEvent[])Enum.GetValues(typeof(eScheduleEvent)));
            long watchedEventID = -1; 

            // 역순환 -  기록된 것 중에 가장 큰 스케줄 인덱스를 가져오기 위해
            scheduleList.Reverse();
            foreach(eScheduleEvent sch in scheduleList)
            {
                if(DataManager.Instance.playerData.WatchedEventIDList.Contains((int)sch))
                {
                    watchedEventID = (long)sch;
                    break;
                }
            }
            return watchedEventID;
        }

        /// <summary> 다음 실행될 예정인 학사일정 이벤트 enum 리턴 </summary>
        public long GetNextScheduleID()
        {
            List<eScheduleEvent> scheduleList = new List<eScheduleEvent>((eScheduleEvent[])Enum.GetValues(typeof(eScheduleEvent)));
            int schIndex = -1;

            if (DataManager.Instance.playerData.WatchedEventIDList.Count == 0)
                return -1;

            // 역순환 -  기록된 것 중에 가장 큰 스케줄ID를 가져오기 위해
            scheduleList.Reverse();
            foreach (eScheduleEvent sch in scheduleList)
            {
                if (DataManager.Instance.playerData.WatchedEventIDList.Contains((int)sch))
                {
                    schIndex = scheduleList.IndexOf(sch);
                    break;
                }
            }

            eScheduleEvent scheduleEvent = scheduleList[schIndex - 1];
            return (long)scheduleEvent;
        }
        #endregion

        /// <summary> 외부에서 이벤트 확인하고 로드할 때 호출 </summary>
        public void CheckEvent()
        {
            EnqueueEventID();
            LoadEventData();
            if (EventQueue.Count > 0)
            {
                IsEventAllFinished = false;
                DataManager.Instance.playerData.CurrentStatus = eStatus.Event;
            }
            else 
            {
                IsEventAllFinished = true;
                DataManager.Instance.playerData.CurrentStatus = eStatus.Main; 
            }
        }

        /// <summary> 이벤트 데이터를 로드해서 대기열에 추가 </summary>
        private void LoadEventData()
        {
            if (EventIDQueue.Count == 0) return;

            while (EventIDQueue.Count > 0)
            {
                EventTitle eventTitle = EventIDQueue.Dequeue();
                Dictionary<long, EventScript> eventScripts = LoadScript(eventTitle.index);

                EventData eventData = new EventData(eventTitle, eventScripts);
                Debug.Log($"EventQueue에 추가된 이벤트: {eventData.eventTitle.EventName} (넣기 전 count : {EventQueue.Count})");
                EventQueue.Enqueue(eventData);
            }
        }

        /// <summary> 이벤트 인덱스에 맞는 이벤트 스크립트 전체 불러오기 </summary>
        private Dictionary<long, EventScript> LoadScript(long eventID)
        {
            return DataManager.Instance.EventScriptDict[eventID];
        }

        /// <summary> 분기 결과 이외 스크립트는 지움 </summary>
        public void DeleteOtherScripts(long startIndex, long? endIndex = null)
        {
            while (nowEventData.eventScripts.ContainsKey(startIndex))
            {
                if (endIndex.HasValue && startIndex >= endIndex.Value) break;
                nowEventData.eventScripts.Remove(startIndex++);
            }
        }

        public void SaveTempApplyEvent(EventTitle eventTitle, bool isEnroll)
        {
            // 여기서 이벤트 참가여부 묻는 이벤트 정보를 저장하도록 하고
            ApplingEvent = eventTitle;
            IsEventApplied = isEnroll;
        }
        /// <summary> 이벤트 참가 여부 저장 </summary>
        /// <param name="isEnroll">첫번째 버튼 : true / 두번째 버튼 : false</param>
        public void ApplyEvents()
        {
            if (ApplingEvent == null) return;

            Debug.Log($"<color=yellow>이벤트{ApplingEvent.EventName}의  다음 이벤트 지원 가능 여부{ApplingEvent.ApplyOption}, 지원 의사: {IsEventApplied}</color>");
            
            if (ApplingEvent.ApplyOption)
            {
                long eventID = ApplingEvent.ApplyEvent;
                Debug.Log($"이 값 false이면 뒷 로그 안나옴 {DataManager.Instance.playerData.AppliedEventsDict.ContainsKey(eventID)}");
                
                // 이미 지원 정보가 저장되어 있다면 함수 리턴
                if (DataManager.Instance.playerData.AppliedEventsDict.ContainsKey(eventID)) return;
                
                // 저장 안되어있다면 추가
                DataManager.Instance.playerData.AppliedEventsDict.TryAdd(eventID, IsEventApplied);

                if (!IsEventApplied) RecordEventResult(eventID); // 엔딩 이력서 표시용 기록

                Debug.Log($"다음의 {eventID}번 이벤트 참가 신청을 {IsEventApplied}로 함");
            }

        }

        /// <summary> 엔딩 이력서에 표시될 이벤트 진행 결과 저장 </summary>
        public void RecordEventResult(long eventID, string str = "미참여")
        {
            string title = DataManager.Instance.GetData<EventTitle>(eventID).EventName;

            if (title.EndsWith("!")) // "!" 지우기
            {
                title = title.Substring(0, title.Length - 1);
                DataManager.Instance.playerData.EventRecordList_etc.Add(new(title, str));
            }
            else // "고사" 지우기
            {
                title = title.Substring(0, title.Length - 2);
                DataManager.Instance.playerData.EventRecordList.Add(new(title, str));
            }
        }
    }
}