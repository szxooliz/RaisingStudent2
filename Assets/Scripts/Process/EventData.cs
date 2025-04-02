using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class EventData
    {
        public EventTitle eventTitle;
        public Dictionary<long, EventScript> eventScripts;

        public EventData(EventTitle _eventTitle, Dictionary<long, EventScript> _eventScripts)
        {
            eventTitle = _eventTitle;
            eventScripts = _eventScripts;
        }
    }
}