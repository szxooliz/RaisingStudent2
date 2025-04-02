using Client;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public class ActivityData
    {
        public eActivityType activityType; // 활동 종류
        public eResultType resultType;     // 활동 결과 실패/성공/대성공

        public List<eStatName> statNames;  // 변경될 스탯 종류
        public List<int> statValues;       // 스탯 증감량
        public float stressValue;          // 스트레스 증감량

        public ActivityData(eActivityType activityType) // 생성자
        {
            this.activityType = activityType;
            resultType = eResultType.MaxCount;

            statNames = new List<eStatName>();
            statValues = new List<int>();
            stressValue = 0;
        }
    }
}