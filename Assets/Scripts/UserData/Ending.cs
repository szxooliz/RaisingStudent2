using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class Ending
    {
        public eEndingName endingName;   // 엔딩 이름
        public bool isUnlocked = false;         // 해금 여부

        public string applicationField = null; // 지원 분야
        public string grade = null;            // 게임 성적
        public List<string> awards = null;     // 기타 이력

        public Ending(eEndingName name, PlayerData _playerData) // 생성자
        {
            endingName = name;
        }
    }
}