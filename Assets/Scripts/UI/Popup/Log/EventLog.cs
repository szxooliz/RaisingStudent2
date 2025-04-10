using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Client
{
    public class EventLog
    {
        int _uid;
        public string title;
        // event&activity 단위를 담을 class
        public List<UnitLog> unitLogs = new();

        public int UID => _uid;

    }
}