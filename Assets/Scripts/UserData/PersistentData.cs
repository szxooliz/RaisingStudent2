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

        public PersistentData() // 생성자
        {
            foreach (eEndingName endingName in Enum.GetValues(typeof(eEndingName)))
            {
                Ending ending = new Ending(endingName);

                // *****더미데이터*****
                ending.awards.Add("A+");
                ending.awards.Add("B0");
                ending.awards.Add("C+");
                ending.awards.Add("D+");
                // *****더미데이터*****

                endingList.Add(ending);
            }
        }
    }

}