using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Data;
using System.Linq;

namespace Client
{
    public partial class EventScript : SheetData
    {
		public long index; // 스크립트 넘버
		public long EventNum; // 이벤트 넘버
		public string Character; // 캐릭터 고유값
		public string Line; // 캐릭터의 대사
		public string Face; // 캐릭터의 감정
		public bool NameTag; // 이름표 사용 여부
		public long SelectIndex; // 선택지 번호
		public long SkipLine; // 스킵라인
		

        public override Dictionary<long, SheetData> LoadData()
        {
            var dataList = new Dictionary<long, SheetData>();

            string ListStr = null;
			int line = 0;
            TextAsset csvFile = Resources.Load<TextAsset>($"CSV/{this.GetType().Name}");
            try
			{            
                string csvContent = csvFile.text;
                string[] lines = csvContent.Split('\n');
                for (int i = 3; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] values = lines[i].Trim().Split(',');
                    line = i;

					if (values.Length <= 0)
						continue;

					if (values[0].Contains("#"))
						continue;

                    EventScript data = new EventScript();

                    
					if(values[0] == "")
					    data.index = default;
					else
					    data.index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.EventNum = default;
					else
					    data.EventNum = Convert.ToInt64(values[1]);
					
					if(values[2] == "")
					    data.Character = default;
					else
					    data.Character = Convert.ToString(values[2]);
					
					if(values[3] == "")
					    data.Line = default;
					else
					    data.Line = Convert.ToString(values[3]);
					
					if(values[4] == "")
					    data.Face = default;
					else
					    data.Face = Convert.ToString(values[4]);
					
					if(values[5] == "")
					    data.NameTag = default;
					else
					    data.NameTag = Convert.ToBoolean(values[5]);
					
					if(values[6] == "")
					    data.SelectIndex = default;
					else
					    data.SelectIndex = Convert.ToInt64(values[6]);
					
					if(values[7] == "")
					    data.SkipLine = default;
					else
					    data.SkipLine = Convert.ToInt64(values[7]);
					
                    
                    dataList[data.index] = data;
                }

                return dataList;
            }
			catch (Exception e)
			{
				Debug.LogError($"{this.GetType().Name}의 {line}전후로 데이터 문제 발생");
				return new Dictionary<long, SheetData>();
			}
        }
    }
}