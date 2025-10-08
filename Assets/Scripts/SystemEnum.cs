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

        public enum eLineType
        {
            NARRATION, SPEAK, RESULT
        }

        #region Turns
        public enum eMonths
        {
            Mar = 3, Apr=4, May=5, Jun=6,
            Sep=9, Oct=10, Nov=11, Dec=12
        }

        public enum eThirds
        {
            First = 1, Second = 2, Third = 0
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
            자체휴강, 수업, 게임, 운동, 동아리
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
            Intro = 0, MidTest_1 = 4, FinTest_1 = 6, SummerVac = 7, 
            MidTest_2 = 10, FinTest_2 = 12
        }

        public enum eRecordType
        {
            Test, ETC
        }
        #endregion

        #region EndingName
        public enum eEndingName
        {
            GraduateStudent, CorporateSI, GameCompany, VirtualYoutuber, ProGamer, HomeProtector, MaxCount
        }
        public enum eEndingNameKor
        {
            대학원생, 대기업SI, 게임회사, 버튜버, 프로게이머, 홈프로텍터, MaxCount
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