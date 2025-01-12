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
    public partial class CharacterFace : SheetData
    {
public long index; // 인덱스
		public string Character; // 캐릭터 고유값
		public string CharacterName; // 캐릭터 이름
		public string basic; // 기본
		public string glad; // 기쁨
		public string sad; // 슬픔
		

        public override Dictionary<long, SheetData> LoadData()
        {
            var dataList = new Dictionary<long, SheetData>();

            string ListStr = null;
			int line = 0;
            TextAsset csvFile = Resources.Load<TextAsset>($"CSV/{this.GetType().Name}");
            try
			{            
                string csvContent = csvFile.text;
                var lines = Regex.Split(csvContent, @"\r?\n");
                for (int i = 3; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] values = Regex.Split(lines[i], @",(?=(?:[^""]*""[^""]*"")*[^""]*$)");
                    line = i;

                    CharacterFace data = new CharacterFace();

                    
					if(values[0] == "")
					    data.index = default;
					else
					    data.index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.Character = default;
					else
					    data.Character = Convert.ToString(values[1]);
					
					if(values[2] == "")
					    data.CharacterName = default;
					else
					    data.CharacterName = Convert.ToString(values[2]);
					
					if(values[3] == "")
					    data.basic = default;
					else
					    data.basic = Convert.ToString(values[3]);
					
					if(values[4] == "")
					    data.glad = default;
					else
					    data.glad = Convert.ToString(values[4]);
					
					if(values[5] == "")
					    data.sad = default;
					else
					    data.sad = Convert.ToString(values[5]);
					

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