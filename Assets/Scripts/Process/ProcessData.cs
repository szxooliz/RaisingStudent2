using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class ProcessData
    {
        public string title; // 제목
        public bool hasChange; // 결과에 스탯 변화가 포함되는지 - 활동/이벤트 마지막에 값 변경해야 할 듯
        //public List<(string name, string line)> processLines; // 현재 프로세스에서 사용된 대사 정보 - 불변값 (캐릭터 이름, 대사)

        public ProcessData()
        {
            title = "";
            hasChange = true;
            //processLines = new List<(string name, string line)>();
        }
    }
}