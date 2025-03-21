using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class UnitLog : MonoBehaviour
    {
        // 로그 말풍선 전용

        // 말하는 캐릭터 이름
        // 말하는 캐릭터 이미지 스프라이트 경로 참조할 거
        // 대사 스트링
        // 토글 접어도 나타낼지 말지 bool

        string charName;
        string charPath;
        string line;
        bool isCollapsable;

        readonly string spritePath = "";
        // EventScript 받아서 가공하는거랑
        // 활동 시에 나타날 대사 or 나레이션 가공하는거 넣기

        public void InitUnitLog(EventScript eventScript)
        {
            charName = eventScript.Character;
            //charPath = 
        }

        public void InitUnitLog(string line, SystemEnum.eLineType eLineType)
        {

        }
    }

}