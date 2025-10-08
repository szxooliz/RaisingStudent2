using System;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    [Serializable]
    public class PlayerData
    {
        public string CharName { get; private set; } = "Comsoon";  // 프로토타입
        public int CurrentTurn { get; set; } = 0;                  // 턴 0~24 - 0: 인트로 이벤트용
        public eMonths CurrentMonth => GetMonth();   // n월 3-6/9-12
        public eThirds CurrentThird => GetTerm(); // a순 상중하

        public List<(string title, string record)> EventRecordList;
        public List<(string title, string record)> EventRecordList_etc;
        public Dictionary<long, bool> AppliedEventsDict;
        public List<long> WatchedEventIDList;
        public event EventHandler OnStatusChanged;

        private eStatus _currentStatus = eStatus.Main;
        public eStatus CurrentStatus
        {
            get => _currentStatus;
            set
            {
                if (_currentStatus != value)
                {
                    Debug.Log($"CurrrentState 변경 감지 : {_currentStatus} -> {value}");

                    _currentStatus = value;
                    OnStatusChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public int[] StatsAmounts { get; } = new int[4];

        private float _stressAmount;
        public float StressAmount
        {
            get => _stressAmount;
            set => _stressAmount = Mathf.Clamp(value, 0, 100);
        }

        eMonths GetMonth()
        {
            int m = CurrentTurn / 3;
            if (m < 4) return (eMonths)(m + 3);
            else return (eMonths)(m + 5);
        }

        eThirds GetTerm()
        {
            int l = CurrentTurn % 3;
            return (eThirds)l;
        }
        public PlayerData() 
        {
            EventRecordList = new(); // 엔딩 이력서 표시용 이벤트 진행 결과 - 성적
            EventRecordList_etc = new(); // 엔딩 이력서 표시용 이벤트 진행 결과 - 기타이력
            AppliedEventsDict = new(); // 이벤트 참여 여부 기록
            WatchedEventIDList = new(); // 진행한 이벤트 아이디만 담아둠
        }
    }
}
