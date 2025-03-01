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
    public partial class EventTitle : SheetData
    {
public long index; // 이벤트 넘버
		public string EventName; // 이벤트 제목
		public long AppearStart; // 등장 시작 턴
		public long AppearEnd; // 등장 종료 턴
		public bool IsEnrolled; // 참가신청 여부
		

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

                    EventTitle data = new EventTitle();

                    
					if(values[0] == "")
					    data.index = default;
					else
					    data.index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.EventName = default;
					else
					    data.EventName = Convert.ToString(values[1]);
					
					if(values[2] == "")
					    data.AppearStart = default;
					else
					    data.AppearStart = Convert.ToInt64(values[2]);
					
					if(values[3] == "")
					    data.AppearEnd = default;
					else
					    data.AppearEnd = Convert.ToInt64(values[3]);
					
					if(values[4] == "")
					    data.IsEnrolled = default;
					else
					    data.IsEnrolled = Convert.ToBoolean(values[4]);
					

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