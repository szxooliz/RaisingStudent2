using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    [System.Serializable]
    public class PersistentData
    {
        public Dictionary<eEndingName, Ending> endingDict = new();

        public PersistentData()
        {}

        /// <summary>
        /// 중복된 엔딩이 있으면 덮어쓰고, 없으면 추가합니다.
        /// </summary>
        public void AddOrUpdateEnding(Ending newEnding)
        {
            if (endingDict.ContainsKey(newEnding.endingName))
            {
                endingDict[newEnding.endingName] = newEnding;
            }
            else
            {
                endingDict.Add(newEnding.endingName, newEnding);
            }
        }
    }

}