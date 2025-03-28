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
    }

}