using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class Ending
    {
        public PlayerData playerData;
        public eEndingName endingName;

        public Ending(eEndingName name, PlayerData _playerData)
        {
            endingName = name;
            this.playerData = _playerData;
        }
    }
}