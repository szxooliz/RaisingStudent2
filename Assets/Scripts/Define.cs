using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    public class Define
    {
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

        public enum Status
        {
            Main, Activity, Event
        }

        #region Turns
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
        #endregion

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

        #region Activity
        public enum ActivityType
        {
            Rest, Class, Game, Workout, Club, MaxCount
        }

        public enum ActivityTypeKor
        {
            Rest, Class, Game, Workout, Club, MaxCount
        }

        public static string GetActivityTypeKor(ActivityType activityType)
        {
            if (activityType == ActivityType.MaxCount) return null;

            string temp = "";
            temp += ((ActivityTypeKor)((int)activityType)).ToString();
            return temp;
        }

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
        #endregion

        public enum EventDataType
        {
            Main, Random
        }

        public enum MainEvents
        {
            Intro, ApplyToHackerton, Hackerton, MidTest_1, SportsDay, FinTest_1, 
            SummerVac, 
            Festival, MidTest_2, Gstar, FinTest_2
        }

        #region EndingName
        public enum EndingName
        {
            GraduateStudent, CorporateSI, GameCompany, VirtualYoutuber, ProGamer, HomeProtector, MaxCount
        }
        public enum EndingNameKor
        {
            대학원생, 대기업SI, 게임회사, 버튜버, 프로게이머, 홈프로텍터, MaxCount
        }
        public static string GetEndingNameKor(EndingName endingName)
        {
            if (endingName == EndingName.MaxCount) return null;

            string temp = "";
            temp += ((EndingNameKor)((int)endingName)).ToString();
            return temp;
        }
        #endregion


        #region UserData
        [System.Serializable]
        public class PlayerData
        {
            public int currentTurn; // 턴 0~23
            public Months currentMonth; // n월 3-6/9-12
            public Thirds currentThird; // a순 상중하
            public List<ProcessData> watchedProcess; // 로그용 - 이미 진행한 or 진행 중인 활동, 이벤트 순서대로 저장
            public List<EventData> watchedEvents; // 이벤트 중복 실행 방지용 - 리스트에 있으면 이미 본 이벤트

            public event EventHandler OnStatusChanged; // 상태 변경에 따라 UI 활성화 하는 용도의 이벤트 핸들러

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

            public int[] statsAmounts; // 스탯 리스트, StatName 열거형 요소와 순서 같음

            [SerializeField] float _stressAmount; // 스트레스 양
            public float stressAmount
            {
                get => _stressAmount;
                set
                {
                    _stressAmount = Mathf.Clamp(value, 0, 100);
                }
            }

            public PlayerData() // 생성자
            {
                currentTurn = 0;
                currentMonth = Months.Mar;
                currentThird = Thirds.First;
                currentStatus = Status.Main;

                statsAmounts = new int[4];
                for (int i = 0; i < statsAmounts.Length; i++)
                {
                    statsAmounts[i] = 0; // 0으로 초기화
                }
                stressAmount = 0;
            }
        }


        [System.Serializable]
        public class PersistentData
        {
            public List<Ending> endingList = new List<Ending>();

            public PersistentData() // 생성자
            {
                foreach (EndingName endingName in Enum.GetValues(typeof(EndingName)))
                {
                    Ending ending = new Ending(endingName);

                    // *****더미데이터*****
                    ending.awards.Add("A");
                    ending.awards.Add("B");
                    ending.awards.Add("C");
                    ending.awards.Add("D");
                    // *****더미데이터*****

                    endingList.Add(ending);
                }
            }
        }
        #endregion

        public class Ending
        {
            public EndingName endingName;   // 엔딩 이름
            public bool isUnlocked;         // 해금 여부
            public string applicationField; // 지원 분야
            public string grade;            // 게임 성적
            public List<string> awards;     // 기타 이력

            public Ending(EndingName name) // 생성자
            {
                endingName = name;
                isUnlocked = false;
                applicationField = "";
                grade = "";
                awards = new List<string>();
            }
        }

        /// <summary>
        /// 활동 하나당 데이터
        /// </summary>
        [System.Serializable]
        public class ActivityData : ProcessData
        {
            public ActivityType activityType; // 활동 종류
            public ResultType resultType;     // 활동 결과 실패/성공/대성공

            public List<StatName> statNames;  // 변경될 스탯 종류
            public List<int> statValues;      // 스탯 증감량

            public float stressValue; // 스트레스 증감량

            public ActivityData() // 생성자
            {
                activityType = ActivityType.MaxCount;
                resultType = ResultType.MaxCount;

                statNames = new List<StatName>();
                statValues = new List<int>();
                stressValue = 0;
            }
        }

        public class EventData : ProcessData
        {
            public long eventIndex;
            public EventDataType eventType; // 필요한가?

            public EventData()
            {
                eventIndex = 0;
                eventType = EventDataType.Main;
            }
        }

        /// <summary>
        /// 활동, 이벤트의 기본 구조
        /// </summary>
        public class ProcessData
        {
            public string title; // 제목
            public bool hasChange; // 결과에 스탯 변화가 포함되는지 - 활동/이벤트 마지막에 값 변경해야 할 듯
            public List<(string name, string line)> processLines; // 현재 프로세스에서 사용된 대사 정보 - 불변값 (캐릭터 이름, 대사)

            public ProcessData()
            {
                title = "";
                hasChange = true;
                processLines = new List<(string name, string line)>();
            }
        }
    }
}
