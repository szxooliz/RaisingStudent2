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

        public string charName;
        public string charPath;
        public string line;
        public bool isCollapsable;

        //public readonly string spritePath = "";

        public UnitLog() { }
    }

}