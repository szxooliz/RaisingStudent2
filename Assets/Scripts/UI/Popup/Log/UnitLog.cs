using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class UnitLog : MonoBehaviour
    {
        public string charName;
        public string charPath;
        public string line;
        public bool isCollapsable;
        public eLineType eLineType;

        public readonly string path = "$Sprites/Character/Profile/";

        // <구분의 기준>
        // 1. 나레이션&선택지 - 꼬리X 말풍선 / 캐릭터 대사 - 꼬리 말풍선 / 스탯 변경 나레이션 - 회색 풍선
        //      -> 이건 eLineType으로 구분
        // 2. 토글 접어도 사라지는지 / 유지되는지
        // 3. 이름표&초상화 있는지 없는지

        /// <summary>
        /// 이벤트 스크립트 중 - 캐릭터 대사와 나레이션용
        /// </summary>
        public UnitLog(eLineType _eLineType, EventScript _eventScript) 
        {
            eLineType = _eLineType;

            if (_eLineType == eLineType.SPEAK)
            {
                charName = Util.GetCharNameKor(_eventScript.Character);
                charPath = path + Util.GetCharBasicSpritePath(_eventScript.Character);
                line = _eventScript.Line;
                isCollapsable = true;
            }
            else if (_eLineType == eLineType.NARRATION)
            {
                line = _eventScript.Line;
                isCollapsable = true;
            }
        }

        /// <summary>
        /// 이벤트 - 결과 & 선택지 / 활동 중 캐릭터 대사 & 결과 
        /// </summary>
        /// <param name="_line"></param>
        public UnitLog(eLineType _eLineType, string _line)
        {
            eLineType = _eLineType;
            line = _line;

            // 이벤트&활동의 결과
            if (eLineType == eLineType.RESULT)
            {
                isCollapsable = false;
            }
            // 활동의 캐릭터 대사
            else if (eLineType == eLineType.SPEAK)
            {
                isCollapsable = true;
                charName = Util.GetCharNameKor(DataManager.Instance.playerData.CharName);
                charPath = path + Util.GetCharBasicSpritePath(DataManager.Instance.playerData.CharName);
            }
            else // NARRATION , 이벤트의 선택지
            {
                isCollapsable = true;
            }
        }


    }

}