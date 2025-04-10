using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class LogManager : Singleton<LogManager>
    {
        public List<EventLog> eventLogList = new();

        //public EventLog GetOrAddEventLog(int _id)
        //{
        //    // UnitLog를 넣어줄 클래스
        //    // 어떤 이벤트 로그에 UnitLog가 포함되는지를 알아야 하거나
        //    // 
        //}
        public void AddUnitLogList(UnitLog unitLog)
        {

        }
    }
}
