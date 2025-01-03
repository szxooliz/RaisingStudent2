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
    public partial class SelectScript : SheetData
    {
public long index; // 스크립트 넘버
		public string SelectNum; // 선택지
		public long MoveLine; // 옮길 라인
		

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

                    SelectScript data = new SelectScript();

                    
					if(values[0] == "")
					    data.index = default;
					else
					    data.index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.SelectNum = default;
					else
					    data.SelectNum = Convert.ToString(values[1]);
					
					if(values[2] == "")
					    data.MoveLine = default;
					else
					    data.MoveLine = Convert.ToInt64(values[2]);
					
                    
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