using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class EventData : ProcessData
    {
        //public long eventIndex;
        //public eEventType eventType; // 필요한가?
        public EventTitle eventTitle;
        public Dictionary<long, EventScript> eventScripts;
        public EventResult eventResult; // 이벤트 결과 있으면 넣고 없으면 null

        public EventData(EventTitle _eventTitle, Dictionary<long, EventScript> _eventScripts)
        {
            title = _eventTitle.EventName;
            eventTitle = _eventTitle;
            eventScripts = _eventScripts;
        }
    }
}