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

        //public string applicationField = null; // 지원 분야
        //public string grade = null;            // 게임 성적
        //public List<string> awards = null;     // 기타 이력

        // PlayerData 안에 플레이 기록(시험 성적, 참여 여부 등) 있으므로 엔딩 데이터 하나에 PlayerData 포함 해야 함
        // 엔딩 객체 생성 될 때 DataManager에서 쓰던 playerData 매핑해주면 될 듯
        public PlayerData playerData;

        public Ending(eEndingName name, PlayerData _playerData) // 생성자
        {
            endingName = name;
            playerData = _playerData;
        }
    }
}