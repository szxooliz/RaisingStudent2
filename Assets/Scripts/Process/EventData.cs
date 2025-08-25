using Client;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        // 복사 생성자 (Deep Copy)
        public EventData(EventData other)
        {
            eventTitle = other.eventTitle;

            // Dictionary 깊은 복사
            eventScripts = new Dictionary<long, EventScript>();
            foreach (var kv in other.eventScripts)
            {
                eventScripts[kv.Key] = kv.Value.Clone(); 
            }
        }
    }
}