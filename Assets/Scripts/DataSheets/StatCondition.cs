using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Client
{
    public partial class StatCondition : SheetData
    {
public long index; // 인덱스
		public long Inteli; // 지력
		public long Otaku; // 덕력
		public long Strength; // 체력
		public long Charming; // 매력
		public long TrueIndex; // 참일 때 인덱스
		public long FalseIndex; // 거짓일 때 인덱스
		
		public SystemEnum.eRecordType RecordType; // 결과 기록 기준
		public string TrueRecord; // 충족 시 이력
		public string FalseRecord; // 미충족 시 이력
		

        public override Dictionary<long, SheetData> LoadData()
        {
            var dataList = new Dictionary<long, SheetData>();

            string ListStr = null;
			int line = 0;
            TextAsset csvFile = Resources.Load<TextAsset>($"CSV/{this.GetType().Name}");
            try
			{            
                string csvContent = csvFile.text;
                string[] lines = Regex.Split(csvContent, @"(?!(?<=(?:(,"")[^""]*))\r?\n)\r?\n");
                for (int i = 3; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] values = Regex.Split(lines[i].Trim(),
                                        @"(?!(?<=(?:(,"")[^""]*)),),");
                    
                    for (int j = 0; j < values.Length; j++)
                    {
                        values[j] = Regex.Replace(values[j], @"^""|""$", "");
                    }

                    line = i;

                    StatCondition data = new StatCondition();

                    
					if(values[0] == "")
					    data.index = default;
					else
					    data.index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.Inteli = default;
					else
					    data.Inteli = Convert.ToInt64(values[1]);
					
					if(values[2] == "")
					    data.Otaku = default;
					else
					    data.Otaku = Convert.ToInt64(values[2]);
					
					if(values[3] == "")
					    data.Strength = default;
					else
					    data.Strength = Convert.ToInt64(values[3]);
					
					if(values[4] == "")
					    data.Charming = default;
					else
					    data.Charming = Convert.ToInt64(values[4]);
					
					if(values[5] == "")
					    data.TrueIndex = default;
					else
					    data.TrueIndex = Convert.ToInt64(values[5]);
					
					if(values[6] == "")
					    data.FalseIndex = default;
					else
					    data.FalseIndex = Convert.ToInt64(values[6]);
					
					if(values[7] == "")
					    data.RecordType = default;
					else
					    data.RecordType = (SystemEnum.eRecordType)Enum.Parse(typeof(SystemEnum.eRecordType), values[7]);
					
					if(values[8] == "")
					    data.TrueRecord = default;
					else
					    data.TrueRecord = Convert.ToString(values[8]);
					
					if(values[9] == "")
					    data.FalseRecord = default;
					else
					    data.FalseRecord = Convert.ToString(values[9]);
					

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