using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Client.SystemEnum;
using System.Linq;

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

                if (DataManager.Instance.playerData.currentTurn == eventTitle.AppearStart)
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

        /// <summary>
        /// 봤던 이벤트로 기록
        /// </summary>
        /// <param name="eventData"></param>
        private void AddWatchedEvent(EventData eventData)
        {
            try
            {
                DataManager.Instance.playerData.watchedEvents.Add(eventData.eventIndex, eventData);
            }
            catch
            {
                Debug.LogError($"{eventData.title} : {eventData.eventIndex}는 이미 등록된 이벤트 인덱스입니다");
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
                if (DataManager.Instance.playerData.watchedEvents.ContainsKey(randomList[i].index))
                {
                    randomList.RemoveAt(i);
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
            return UnityEngine.Random.Range(0, 101) <= RANDOM_PROB;
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
                AddWatchedEvent(eventData);
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
            int tempIndex = eventType == eEventType.Random ? RANDOM_SCRIPT_THRESHOLD : 0;

            while (true)
            {
                try
                {
                    EventScript eventScript = DataManager.Instance.GetData<EventScript>(tempIndex);

                    if (eventScript.EventNum == eventID)
                    {
                        // EventUI.cs로 옮겨서, 유저가 선택지를 선택할 타이밍에 실행되도록 해주세요!
                        //if (eventScript.BranchType == eBranchType.Choice)
                        //{
                        //    int nextLineIndex = GetNextEventScriptIndex((int)eventScript.BranchIndex);
                        //    eventScript = DataManager.Instance.GetData<EventScript>(nextLineIndex);
                        //}

                        eventScripts.Add(eventScript);
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

        private int GetNextEventScriptIndex(int branchIndex)
        {
            // SelectScript.csv에서 branchIndex에 맞는 C열의 옮길 라인 값을 가져옴
            try
            {
                SelectScript selectScript = DataManager.Instance.GetData<SelectScript>(branchIndex);
                return (int)selectScript.MoveLine;
            }
            catch
            {
                Debug.LogError("SelectScript에서 다음 이벤트 스크립트 인덱스를 찾을 수 없습니다.");
                return -1; // 예외 처리, 잘못된 인덱스일 경우
            }
        }
    }
}