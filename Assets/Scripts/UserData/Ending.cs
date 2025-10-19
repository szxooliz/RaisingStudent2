using System;
using Newtonsoft.Json;
using static Client.SystemEnum;

namespace Client
{
    public class Ending
    {
        [JsonProperty] public PlayerData playerData;
        [JsonProperty] public eEndingName EndingName;

        public Ending(eEndingName name, PlayerData _playerData)
        {
            EndingName = name;
            playerData = _playerData;
        }
    }
}