using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Client
{
    public class LogCluster
    {
        int _uid = 0;
        public string title;
        // event&activity 단위를 담을 class
        public List<UnitLog> unitLogs = new();

        public int UID => _uid;
        public LogCluster(string title)
        {
            _uid = LogManager.Instance.GetNextID();
            this.title = title;

        }
        public void AddUnitLogList(UnitLog unitLog)
        {
            unitLogs.Add(unitLog);
            Debug.Log($"{this.title}에 로그 추가 : {unitLog.charName} {unitLog.line}");
        }


    }
}