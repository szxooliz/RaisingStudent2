using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    public class Define
    {
        public enum eScene
        {
            Base, Title
        }

        public enum eSound
        {
            SFX, BGM,
            MaxCount
        }

        public enum eUIEvent
        {
            Click,
            Drag,
        }

        public enum eStatus
        {
            Main, Activity, Event
        }

        #region Turns
        public enum eMonths
        {
            Mar = 3, Apr=4, May=5, Jun=6,
            Sep=9, Oct=10, Nov=11, Dec=12
        }

        public enum eThirds
        {
            First, Second, Third
        }
        public enum eThirdsKor
        {
            상순, 중순, 하순
        }
        public static string GetThirdsKor(eThirds third)
        {
            string temp = "";
            temp += ((eThirdsKor)((int)third)).ToString();
            return temp;
        }
        #endregion

        #region StatName
        public enum eStatName
        {
            Inteli, Otaku, Strength, Charming, MaxCount
        }
        public enum eStatNameKor
        {
            지력, 덕력, 체력, 매력
        }
        public static string GetStatNameKor(eStatName statName)
        {
            if (statName == eStatName.MaxCount) return null;

            string temp = "";
            temp += ((eStatNameKor)((int)statName)).ToString();
            return temp;
        }
        #endregion

        #region Activity
        public enum eActivityType
        {
            Rest, Class, Game, Workout, Club, MaxCount
        }

        public enum eActivityTypeKor
        {
            Rest, Class, Game, Workout, Club, MaxCount
        }

        public static string GetActivityTypeKor(eActivityType activityType)
        {
            if (activityType == eActivityType.MaxCount) return null;

            string temp = "";
            temp += ((eActivityTypeKor)((int)activityType)).ToString();
            return temp;
        }

        public enum eResultType
        {
            BigSuccess, Success, Failure, MaxCount
        }
        public enum eResultTypeKor
        {
            대성공, 성공, 실패
        }

        public static string GetResultTypeKor(eResultType resultType)
        {
            if (resultType == eResultType.MaxCount) return null;

            string temp = "";
            temp += ((eResultTypeKor)((int)resultType)).ToString();
            return temp;
        }
        #endregion

        public enum eEventType
        {
            Main, Random
        }

        public enum eMainEvents
        {
            Intro, ApplyToHackerton, Hackerton, MidTest_1, SportsDay, FinTest_1, 
            SummerVac, 
            Festival, MidTest_2, Gstar, FinTest_2
        }

        #region EndingName
        public enum eEndingName
        {
            HomeProtector, ProGamer, VirtualYoutuber, GameCompany, CorporateSI, GraduateStudent, MaxCount
        }
        public enum eEndingNameKor
        {
            홈프로텍터, 프로게이머, 버튜버, 게임회사, 대기업SI, 대학원생, MaxCount
        }
        public static string GetEndingNameKor(eEndingName endingName)
        {
            if (endingName == eEndingName.MaxCount) return null;

            string temp = "";
            temp += ((eEndingNameKor)((int)endingName)).ToString();
            return temp;
        }
        #endregion


        #region UserData
        [System.Serializable]
        public class PlayerData
        {
            public string charName; // 현재 플레이 중인 캐릭터 이름
            public int currentTurn; // 턴 0~23
            public eMonths currentMonth; // n월 3-6/9-12
            public eThirds currentThird; // a순 상중하
            public List<ProcessData> watchedProcess; // 로그용 - 이미 진행한 or 진행 중인 활동, 이벤트 순서대로 저장
            public List<EventData> watchedEvents; // 이벤트 중복 실행 방지용 - 리스트에 있으면 이미 본 이벤트

            public event EventHandler OnStatusChanged; // 상태 변경에 따라 UI 활성화 하는 용도의 이벤트 핸들러

            [SerializeField] eStatus _currentStatus; // 현재 상태 
            public eStatus currentStatus
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
                charName = "Comsoon"; // 프로토타입
                currentTurn = 0;
                currentMonth = eMonths.Mar;
                currentThird = eThirds.First;
                currentStatus = eStatus.Main;

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
                foreach (eEndingName endingName in Enum.GetValues(typeof(eEndingName)))
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
            public eEndingName endingName;   // 엔딩 이름
            public bool isUnlocked;         // 해금 여부
            public string applicationField; // 지원 분야
            public string grade;            // 게임 성적
            public List<string> awards;     // 기타 이력

            public Ending(eEndingName name) // 생성자
            {
                endingName = name;
                isUnlocked = false;
                applicationField = "";
                grade = "";
                awards = new List<string>();
            }
        }
    }
}
