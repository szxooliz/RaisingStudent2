using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class ActivityData : ProcessData
    {
        public eActivityType activityType; // 활동 종류
        public eResultType resultType;     // 활동 결과 실패/성공/대성공

        public List<eStatName> statNames;  // 변경될 스탯 종류
        public List<int> statValues;      // 스탯 증감량

        public float stressValue; // 스트레스 증감량

        public ActivityData() // 생성자
        {
            activityType = eActivityType.MaxCount;
            resultType = eResultType.MaxCount;

            statNames = new List<eStatName>();
            statValues = new List<int>();
            stressValue = 0;
        }
    }
}