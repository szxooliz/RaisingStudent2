using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class EventData : ProcessData
    {
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