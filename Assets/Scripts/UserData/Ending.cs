using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class Ending
    {
        public PlayerData playerData;
        public eEndingName endingName;   // 엔딩 이름
        public bool isUnlocked = true;  // 해금 여부

        public Ending(eEndingName name, PlayerData _playerData) // 생성자
        {
            endingName = name;
            this.playerData = _playerData;
        }
    }
}