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
        public List<Ending> endingList = new List<Ending>();

        public PersistentData()
        {}

        /// <summary>
        /// 중복된 엔딩이 있으면 덮어쓰고, 없으면 추가합니다.
        /// </summary>
        public void AddOrUpdateEnding(Ending newEnding)
        {
            int existingIndex = endingList.FindIndex(e => e.endingName == newEnding.endingName);

            if (existingIndex >= 0)
            {
                // 이미 엔딩이 있는 경우
                endingList[existingIndex] = newEnding;
            }
            else
            {
                endingList.Add(newEnding);
            }
        }
    }

}