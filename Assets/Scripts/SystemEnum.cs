using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    public class SystemEnum
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

        public enum eBranchType
        {
            Choice, Condition
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

        #region Event
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
        #endregion

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
    }
}