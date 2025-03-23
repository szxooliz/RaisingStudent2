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
        public eMonths CurrentMonth { get; set; } = eMonths.Mar;   // n월 3-6/9-12
        public eThirds CurrentThird { get; set; } = eThirds.First; // a순 상중하

        public List<(string title, string record)> EventRecordList = new(); // 엔딩 이력서 표시용 이벤트 진행 결과
        public Dictionary<long, EventData> WatchedEventsDict = new();   // 이벤트 중복 방지용 기록
        public Dictionary<long, bool> AppliedEventsDict = new();        // 이벤트 참여 여부 기록

        public event EventHandler OnStatusChanged;

        private eStatus _currentStatus = eStatus.Main;
        public eStatus CurrentStatus
        {
            get => _currentStatus;
            set
            {
                if (_currentStatus != value)
                {
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
        public PlayerData() { }
    }
}
