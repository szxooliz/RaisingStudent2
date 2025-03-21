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

        public void Initialize()
        {
            foreach (eEndingName endingName in Enum.GetValues(typeof(eEndingName)))
            {
                //MARK-: public PersistentData()에서 DataManager.Instance.playerData 접근 시 오류 발생 -> Initialize() 함수 따로 생성
                Ending ending = new Ending(endingName, DataManager.Instance.playerData);
                endingList.Add(ending);
            }
        }
    }

}