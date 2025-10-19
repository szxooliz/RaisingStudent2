using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using static Client.SystemEnum;

namespace Client
{
    [System.Serializable]
    public class PersistentData
    {
        [JsonProperty]
        public Dictionary<eEndingName, Ending> EndingDict { get; set; } = new();


        public PersistentData()
        {}

        /// <summary>
        /// 중복된 엔딩이 있으면 덮어쓰고, 없으면 추가합니다.
        /// </summary>
        public void AddOrUpdateEnding(Ending newEnding)
        {
            if (EndingDict.ContainsKey(newEnding.EndingName))
            {
                EndingDict[newEnding.EndingName] = newEnding;
            }
            else
            {
                EndingDict.Add(newEnding.EndingName, newEnding);
            }
        }
    }

}