using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    public class Define
    {
        // 추가: PersistentData(엔딩 목록 등)

        public enum Scene
        {
            Base, Title
        }

        public enum Sound
        {
            SFX, BGM,
            MaxCount
        }

        public enum UIEvent
        {
            Click,
            Drag,
        }

        public enum Months
        {
            Mar = 3, Apr=4, May=5, Jun=6,   
            Sep=9, Oct=10, Nov=11, Dec=12
        }

        public enum Thirds
        {
            First, Second, Third
        }
        public enum ThirdsKor
        {
            상순, 중순, 하순
        }
        public static string GetThirdsKor(Thirds third)
        {
            string temp = "";
            temp += ((ThirdsKor)((int)third)).ToString();
            return temp;
        }

        #region StatName
        public enum StatName
        {
            Inteli, Otaku, Strength, Charming, MaxCount
        }
        public enum StatNameKor
        {
            지력, 덕력, 체력, 매력
        }

        public static string GetStatNameKor(StatName statName)
        {
            if (statName == StatName.MaxCount) return null;

            string temp = "";
            temp += ((StatNameKor)((int)statName)).ToString();
            return temp;
        }
        #endregion

        public enum ResultType
        {
            BigSuccess, Success, Failure, MaxCount
        }
        public enum ResultTypeKor
        {
            대성공, 성공, 실패
        }

        public static string GetResultTypeKor(ResultType resultType)
        {
            if (resultType == ResultType.MaxCount) return null;

            string temp = "";
            temp += ((ResultTypeKor)((int)resultType)).ToString();
            return temp;
        }
        public enum ActivityType
        {
            Rest, Class, Game, Workout, Club, MaxCount
        }

        public enum EventDataType
        {
            Main, Sub, Conditioned
        }


        public enum Status
        {
            Main, Activity, Event
        }

        public enum EndingName
        {

        }

        [System.Serializable]
        public class PlayerData
        {
            public int currentTurn; // 턴 0~23
            public Months currentMonth; // n월 3-6/9-12
            public Thirds currentThird; // a순 상중하

            public event EventHandler OnStatusChanged; // 상태 변경에 맞추어 UI 활성화
            [SerializeField] Status _currentStatus; // 현재 상태 
            public Status currentStatus
            {
                get => _currentStatus;
                set
                {
                    _currentStatus = value;
                    OnStatusChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public int[] statsAmounts; // 스탯 리스트
            [SerializeField] float _stressAmount; // 스트레스
            public float stressAmount
            {
                get => _stressAmount;
                set
                {
                    _stressAmount = Mathf.Clamp(value, 0, 100);
                }
            }

            public PlayerData()
            {
                currentTurn = 0;
                currentMonth = Months.Mar;
                currentThird = Thirds.First;
                currentStatus = Status.Main;

                statsAmounts = new int[4];
                for (int i = 0; i < statsAmounts.Length; i++)
                {
                    statsAmounts[i] = 0; // 명시적으로 0으로 초기화
                }

                stressAmount = 0;
            }
        }

        [System.Serializable]
        public class ActivityData
        {
            public ActivityType activityType;
            public ResultType resultType;
            public StatName statName1;
            public StatName statName2;

            public int stat1Value;
            public int stat2Value;
            public float stressValue;

            public ActivityData()
            {
                activityType = ActivityType.MaxCount;
                resultType = ResultType.MaxCount;
                statName1 = StatName.MaxCount;
                statName2 = StatName.MaxCount;

                stat1Value= 0;
                stat2Value= 0;
                stressValue = 0;
            }
        }

    }
}
