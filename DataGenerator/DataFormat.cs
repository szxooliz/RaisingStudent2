using System;
using System.Collections.Generic;
using System.Text;

namespace DataGenerator
{
    class DataFormat
    {
        // {0} 자료형
        // {1} 변수명
        // {2} 설명
        public static string dataRegisterFormat =
@"public {0} {1}; // {2}";

        // {0} 자료형
        // {1} 변수명
        // {2} 설명
        public static string dataRegisterListFormat =
@"public List<{0}> {1}; // {2}";

        // {0} : row index
        // {1} : 자료형 이름
        // {2} : 자료형 변환
        public static string dataParseFomat =
@"
if(values[{0}] == """")
    data.{1} = default;
else
    data.{1} = Convert.{2}(values[{0}]);";

        // {0} : row index
        // {1} : 자료형 이름
        // {2} : 자료형 변환
        public static string dataParseListFormat =
@"
ListStr = values[{0}].Replace('[',' ');
ListStr = ListStr.Replace(']', ' ');
var {1}Data = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.{2}(x)).ToList();
data.{1} = {1}Data;";

        public static string dataEnumRegisterFormat =
@"
public SystemEnum.{0} {1}; // {2}";

        // {0} : row index
        // {1} : 자료형 이름
        // {2} : Enum 자료형
        public static string dataEnumParseFomat =
@"
if(values[{0}] == """")
    data.{1} = default;
else
    data.{1} = (SystemEnum.{2})Enum.Parse(typeof(SystemEnum.{2}), values[{0}]);";

        // {0} : 엑셀 이름 (ex: TestData)
        // {1} : 자료형들 (ex: int a)
        // {2} : 파싱
        public static string dataFormat =
@"using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Client
{{
    public partial class {0} : SheetData
    {{
{1}

        public override Dictionary<long, SheetData> LoadData()
        {{
            var dataList = new Dictionary<long, SheetData>();

            string ListStr = null;
			int line = 0;
            TextAsset csvFile = Resources.Load<TextAsset>($""CSV/{{this.GetType().Name}}"");
            try
			{{            
                string csvContent = csvFile.text;
                string[] lines = Regex.Split(csvContent, @""(?!(?<=(?:(,"""")[^""""]*))\r?\n)\r?\n"");
                for (int i = 3; i < lines.Length; i++)
                {{
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] values = Regex.Split(lines[i].Trim(),
                                        @""(?!(?<=(?:(,"""")[^""""]*)),),"");
                    
                    for (int j = 0; j < values.Length; j++)
                    {{
                        values[j] = Regex.Replace(values[j], @""^""""|""""$"", """");
                    }}

                    line = i;

                    {0} data = new {0}();

                    {2}

                    dataList[data.index] = data;
                }}

                return dataList;
            }}
			catch (Exception e)
			{{
				Debug.LogError($""{{this.GetType().Name}}의 {{line}}전후로 데이터 문제 발생"");
				return new Dictionary<long, SheetData>();
			}}
        }}
    }}
}}";
    }
}