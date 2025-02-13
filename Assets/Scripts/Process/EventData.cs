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
        public List<EventScript> eventScripts;

        public EventData(long _eventIndex, eEventType _eventType, List<EventScript> _eventScripts)
        {
            eventIndex = 0;
            eventType = eEventType.Main;
            eventScripts = _eventScripts;
        }
    }
}