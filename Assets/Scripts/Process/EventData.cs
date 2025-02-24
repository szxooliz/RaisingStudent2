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

        public EventData(long _eventIndex, eEventType _eventType, Dictionary<long, EventScript> _eventScripts)
        {
            eventIndex = _eventIndex;
            eventType = _eventType;
            eventScripts = _eventScripts;
        }
    }
}