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
            BGM_Main,        // 메인 BGM
            SFX_Positive,    // 긍정 효과음
            SFX_BigSuccess,  // 대성공 효과음
            SFX_DialogClick, // 대화창 클릭 효과음
            SFX_Negative,    // 부정 효과음
            SFX_Success,     // 성공 효과음
            SFX_Failure,     // 실패 효과음
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
            None, Choice, Condition
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
        public enum eStatNameAll
        {
            Inteli, Otaku, Strength, Charming, Stress, MaxCount
        }
        public enum eStatNameAllKor
        {
            지력, 덕력, 체력, 매력, 스트레스
        }
        public static string GetStatNameAllKor(eStatNameAll statName)
        {
            if (statName == eStatNameAll.MaxCount) return null;

            string temp = "";
            temp += ((eStatNameAllKor)((int)statName)).ToString();
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

        public enum eScheduleEvent
        {
            Intro = 0, MidTest_1 = 3, FinTest_1 = 5, SummerVac = 6, 
            MidTest_2 = 8, FinTest_2 = 9
        }

        public enum eRecordType
        {
            Test, ETC
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