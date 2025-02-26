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
    public partial class SelectScript : SheetData
    {
public long index; // 선택지 번호
		public string Selection1; // 선택지1 
		public string Selection2; // 선택지2
		public long MoveLine1; // 옮길 라인1
		public long MoveLine2; // 옮길 라인2
		

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

                    SelectScript data = new SelectScript();

                    
					if(values[0] == "")
					    data.index = default;
					else
					    data.index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.Selection1 = default;
					else
					    data.Selection1 = Convert.ToString(values[1]);
					
					if(values[2] == "")
					    data.Selection2 = default;
					else
					    data.Selection2 = Convert.ToString(values[2]);
					
					if(values[3] == "")
					    data.MoveLine1 = default;
					else
					    data.MoveLine1 = Convert.ToInt64(values[3]);
					
					if(values[4] == "")
					    data.MoveLine2 = default;
					else
					    data.MoveLine2 = Convert.ToInt64(values[4]);
					

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