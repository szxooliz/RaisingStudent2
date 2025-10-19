using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static Client.SystemEnum;

namespace Client
{
    [Serializable]
    public class PlayerData
    {
        [JsonProperty] public string CharName { get; set; } = "Comsoon";
        [JsonProperty] public int CurrentTurn { get; set; } = 0;

        [JsonIgnore] public eMonths CurrentMonth => GetMonth();
        [JsonIgnore] public eThirds CurrentThird => GetTerm();

        [JsonProperty] public List<EventRecord> EventRecordList { get; set; } = new();
        [JsonProperty] public List<EventRecord> EventRecordList_etc { get; set; } = new();
        [JsonProperty] public Dictionary<long, bool> AppliedEventsDict { get; set; } = new();
        [JsonProperty] public List<long> WatchedEventIDList { get; set; } = new();

        public event EventHandler OnStatusChanged;

        [JsonProperty]
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

        [JsonProperty] public int[] StatsAmounts { get; set; } = new int[4];

        private float _stressAmount;
        [JsonProperty]
        public float StressAmount
        {
            get => _stressAmount;
            set => _stressAmount = Mathf.Clamp(value, 0, 100);
        }

        private eStatus _currentStatus = eStatus.Main;

        private eMonths GetMonth()
        {
            int m = CurrentTurn / 3;
            return (m < 4) ? (eMonths)(m + 3) : (eMonths)(m + 5);
        }

        private eThirds GetTerm()
        {
            int l = CurrentTurn % 3;
            return (eThirds)l;
        }
    }

    [Serializable]
    public class EventRecord
    {
        public string Title { get; set; }
        public string Record { get; set; }

        public EventRecord() { }

        public EventRecord(string title, string record)
        {
            Title = title;
            Record = record;
        }
    }
}
