using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Client.Define;

namespace Client
{
    public class EventManager : Singleton<EventManager>
    {
        int eventProb = 50; // 랜덤 이벤트 등장 확률

        public Queue<(long ID, string title)> eventIDQueue = new Queue<(long, string)>();
        public Queue<EventData> eventQueue = new Queue<EventData>();

        #region Singleton
        EventManager() { }
        #endregion

        /*
        /// <summary>
        /// 활동 이후 이벤트 여부 체크
        /// </summary>
        public void CheckEvent()
        {
            switch (DataManager.Instance.playerData.currentTurn)
            {
                case 0: // 개강 - 현재 씬에서만 테스트용
                    // currentEventID = (int)Define.Instance.MainEventName.Intro;
                    // ShowEvent(currentEventID);
                    Debug.Log((int)Define.MainEvents.Intro);
                    break;
                case 1: // 테스트용
                    currentEventID = (int)Define.MainEvents.Intro;
                    ShowEvent();
                    break;
                case 2: // 해커톤
                    currentEventID = (int)Define.MainEvents.Hackerton;
                    ShowEvent();
                    break;
                case 5: // 중간고사
                    currentEventID = (int)Define.MainEvents.MidTest_1;
                    ShowEvent();
                    break;
                case 6: // 체육대회
                    currentEventID = (int)Define.MainEvents.SportsDay;
                    ShowEvent();
                    break;
                case 11: // 기말고사
                    currentEventID = (int)Define.MainEvents.FinTest_1;
                    ShowEvent();
                    break;
                case 13: // 축제
                    currentEventID = (int)Define.MainEvents.Festival;
                    ShowEvent();
                    break;
                case 17: // 중간고사
                    currentEventID = (int)Define.MainEvents.MidTest_2;
                    ShowEvent();
                    break;
                case 19: // 지스타
                    currentEventID = (int)Define.MainEvents.Gstar;
                    ShowEvent();
                    break;
                case 23: // 기말고사
                    currentEventID = (int)Define.MainEvents.FinTest_2;
                    ShowEvent();
                    break;
                default:
                    // 해당 없을 시 다시 메인 화면으로 전환
                    DataManager.Instance.playerData.currentStatus = Define.Status.Main;
                    break;
            }

        }*/


        /// <summary>
        /// 등장 조건에 맞춰 실행해야 하는 이벤트 인덱스 가져오기
        /// </summary>
        void GetCurrentEventID()
        {
            Debug.Log($"현재 턴 : {DataManager.Instance.playerData.currentTurn}");

            // 메인 이벤트 가져오기
            for (int i = 0; i <= 10; i++)
            {
                EventTitle eventTitle = DataManager.Instance.GetData<EventTitle>(i);

                if (DataManager.Instance.playerData.currentTurn == eventTitle.AppearStart)
                {
                    Debug.Log($"Queue에 추가된 이벤트 : {eventTitle.EventName}");
                    eventIDQueue.Enqueue((eventTitle.index, eventTitle.EventName));
                }
            }

            // 여기서 랜덤 이벤트 유무가 50퍼 확률로 정해지는건지?
            // 랜덤 이벤트 가져오기
            for (int i = 100; i <= 126; i++)
            {
                EventTitle eventTitle = DataManager.Instance.GetData<EventTitle>(i);

                // 등장 가능 턴이면 랜덤 확률 돌리기?
                if (DataManager.Instance.playerData.currentTurn >= eventTitle.AppearStart 
                    && DataManager.Instance.playerData.currentTurn <= eventTitle.AppearEnd)
                {
                    // 임시 : 랜덤 이벤트 등장 확률 50%
                    int prob = UnityEngine.Random.Range(0, 100);
                    if (prob <= eventProb)
                    {
                        Debug.Log($"Queue에 추가된 이벤트 : {eventTitle.EventName}");
                        eventIDQueue.Enqueue((eventTitle.index, eventTitle.EventName));
                        return; // 일단 한번에 랜덤 이벤트 하나만 추가
                    }
                }
            }
        }

        /// <summary>
        /// 이벤트 스크립트 세트로 가져오기
        /// </summary>
        void GetEventData()
        {
            if (eventIDQueue.Count == 0) return;

            (long ID, string title) standByEvent = eventIDQueue.Dequeue();
            long eventID = standByEvent.ID;
            string eventTitle = standByEvent.title;

            eEventType eventType = eEventType.Main;
            List<EventScript> eventScripts = new List<EventScript>();

            // 인덱스 수치에 따라 이벤트 타입 설정
            if (eventID > 100) eventType = eEventType.Random;

            // 이벤트 인덱스에 맞는 이벤트 스크립트 불러오기
            LoadScript(eventID, eventScripts);

            // EventData 구조로 정보 연결해서 Queue에 저장
            EventData eventData = new EventData(eventID, eventType, eventScripts);
            eventData.title = eventTitle;
            eventQueue.Enqueue(eventData);
        }

        /// <summary>
        /// 현재 턴에 해당되는 이벤트 로드 - 외부에서 이벤트 체크 시 호출
        /// </summary>
        public void CheckAbleEvent()
        {
            GetCurrentEventID();
            GetEventData();

            if (eventQueue.Count > 0) ShowEvent();
        }

        /// <summary>
        /// 이벤트 페이즈로 넘어감
        /// </summary>
        public void ShowEvent()
        {
            DataManager.Instance.playerData.currentStatus = eStatus.Event;
        }

        /// <summary>
        /// 이벤트 인덱스에 맞는 이벤트 스크립트 불러오기
        /// </summary>
        void LoadScript(long _eventID, List<EventScript> _eventScriptList)
        {
            int tempIndex = 0;

            while (true)
            {
                EventScript eventScript = DataManager.Instance.GetData<EventScript>(tempIndex++);

                if (eventScript == null)
                    break;

                if (eventScript.EventNum == _eventID)
                {
                    _eventScriptList.Add(eventScript);
                }
            }
        }
    }
}