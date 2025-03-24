using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class LogManager : Singleton<LogManager>
    {
        private List<ProcessData> processList = new();

        #region 활동용
        public UnitLog SetUnitLog(string line, bool isLine)
        {
            UnitLog unitLog = new();
            unitLog.line = line;
            unitLog.isCollapsable = isLine; // 대사라면, 접을 수 있는 프리팹
            return unitLog;
        }
        #endregion

        #region 이벤트용
        // 이벤트 - 캐릭터 대사, 나레이션
        public UnitLog SetUnitLog(EventScript eventScript)
        {
            UnitLog unitLog = new();
            if (eventScript.Character == SystemEnum.eLineType.NARRATION.ToString())
            {
                unitLog.line = eventScript.Line;
                unitLog.isCollapsable = true;
            }
            else
            {
                unitLog.charName = eventScript.Character;
                unitLog.line = eventScript.Line;
                unitLog.isCollapsable = false;
                //unitLog.charPath = 
                // TODO : 캐릭터 이름 넣으면 이미지 경로 가져오는 함수 util에 만들기
            }
            return unitLog;
        }
        // 이벤트 - 결과
        public UnitLog SetUnitLog(string line)
        {
            UnitLog unitLog = new();
            return unitLog;
        }

        // 이벤트 - 선택지
        public UnitLog SetUnitLog()
        {
            UnitLog unitLog = new();
            return unitLog;
        }
        #endregion

    }
}
