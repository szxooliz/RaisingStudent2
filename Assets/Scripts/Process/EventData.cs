using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class EventData : ProcessData
    {
        public long eventIndex;
        public eEventType eventType; // 필요한가?
        public Dictionary<long, EventScript> eventScripts;
        public EventResult eventResult; // 이벤트 결과 있으면 넣고 없으면 null

        public EventData(long _eventIndex, eEventType _eventType, Dictionary<long, EventScript> _eventScripts)
        {
            eventIndex = _eventIndex;
            eventType = _eventType;
            eventScripts = _eventScripts;
        }
    }
}