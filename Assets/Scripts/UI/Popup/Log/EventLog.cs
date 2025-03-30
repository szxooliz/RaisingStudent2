using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class EventLog
    {
        public string title;
        // event&activity 단위를 담을 class
        public List<UnitLog> unitLogs = new();

    }
}