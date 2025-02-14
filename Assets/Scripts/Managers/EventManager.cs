using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Client.SystemEnum;

namespace Client
{
    public class EventManager : Singleton<EventManager>
    {
        #region constant
        private const int MAIN_COUNT = 10; // 메인 이벤트 개수

        private const int RANDOM_THRESHOLD = 100;         // 랜덤 이벤트 ID 시작 인덱스
        private const int RANDOM_SCRIPT_THRESHOLD = 1000; // 랜덤 이벤트 스크립트 시작 인덱스
        private const int RANDOM_COUNT = 26;              // 랜덤 이벤트 개수
        private const int RANDOM_PROB = 50;               // 랜덤 이벤트 등장 확률
        #endregion

        public Queue<(long ID, string title)> EventIDQueue { get; } = new Queue<(long, string)>();
        public Queue<EventData> EventQueue { get; } = new Queue<EventData>();

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

                if (DataManager.Instance.playerData.currentTurn == eventTitle.AppearStart)
                {
                    EventIDQueue.Enqueue((eventTitle.index, eventTitle.EventName));
                }
            }

            // 랜덤 이벤트 가져오기
            for (int i = RANDOM_THRESHOLD; i <= RANDOM_THRESHOLD + RANDOM_COUNT; i++)
            {
                EventTitle eventTitle = DataManager.Instance.GetData<EventTitle>(i);

                if (IsInTurnRange(eventTitle) && TriggerRandomEvent())
                {
                    EventIDQueue.Enqueue((eventTitle.index, eventTitle.EventName));
                    break; // 한 턴에 하나의 랜덤 이벤트만 추가
                }
            }
        }

        /// <summary>
        /// 등장 가능 범위 내의 이벤트인지 확인
        /// </summary>
        /// <param name="eventTitle"></param>
        /// <returns></returns>
        private bool IsInTurnRange(EventTitle eventTitle)
        {
            int currentTurn = DataManager.Instance.playerData.currentTurn;
            return currentTurn >= eventTitle.AppearStart && currentTurn <= eventTitle.AppearEnd;
        }

        /// <summary>
        /// 랜덤 이벤트 자체 발생 확률 계산
        /// </summary>
        /// <returns></returns>
        private bool TriggerRandomEvent()
        {
            return UnityEngine.Random.Range(0, 100) <= RANDOM_PROB;
        }

        /// <summary>
        /// 이벤트 데이터를 로드해서 대기열에 추가
        /// </summary>
        private void LoadEventData()
        {
            Debug.Log($"eventIDQueue에서 전달받아야 할 이벤트 개수 : {EventIDQueue.Count}");

            if (EventIDQueue.Count == 0) return;

            while (EventIDQueue.Count > 0)
            {
                var (eventID, eventTitle) = EventIDQueue.Dequeue();
                eEventType eventType = (eventID > RANDOM_THRESHOLD) ? eEventType.Random : eEventType.Main;
                List<EventScript> eventScripts = LoadScript(eventID, eventType);

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
            DataManager.Instance.playerData.currentStatus = eStatus.Event;
        }

        /// <summary>
        /// 이벤트 인덱스에 맞는 이벤트 스크립트 불러오기
        /// </summary>
        private List<EventScript> LoadScript(long eventID, eEventType eventType)
        {
            List<EventScript> eventScripts = new List<EventScript>();
            int startIndex = eventType == eEventType.Random ? RANDOM_SCRIPT_THRESHOLD : 0;

            for (int i = startIndex; ; i++)
            {
                if (!TryGetScript(i, eventID, out EventScript eventScript))
                    break;
                eventScripts.Add(eventScript);
            }

            Debug.Log($"이벤트 {eventID}의 스크립트 개수: {eventScripts.Count}");
            return eventScripts;
        }

        /// <summary>
        /// 스크립트 로드
        /// </summary>
        /// <param name="index"></param>
        /// <param name="eventID"></param>
        /// <param name="eventScript"></param>
        /// <returns></returns>
        private bool TryGetScript(int index, long eventID, out EventScript eventScript)
        {
            try
            {
                eventScript = DataManager.Instance.GetData<EventScript>(index);
                return eventScript.EventNum == eventID;
            }
            catch
            {
                eventScript = null;
                return false;
            }
        }
    }
}