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
    public partial class EndingScript : SheetData
    {
public long index; // 스크립트 넘버
		public long EndingNum; // 엔딩 넘버
		public string Character; // 캐릭터 고유값
		public string Line; // 캐릭터의 대사
		public string Face; // 캐릭터의 감정
		public bool NameTag; // 이름표 사용 여부
		public bool HasIllust; // 일러스트 등장 여부
		public string Background; // 배경 설정
		

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

                    EndingScript data = new EndingScript();

                    
					if(values[0] == "")
					    data.index = default;
					else
					    data.index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.EndingNum = default;
					else
					    data.EndingNum = Convert.ToInt64(values[1]);
					
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
					    data.HasIllust = default;
					else
					    data.HasIllust = Convert.ToBoolean(values[6]);
					
					if(values[7] == "")
					    data.Background = default;
					else
					    data.Background = Convert.ToString(values[7]);
					

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