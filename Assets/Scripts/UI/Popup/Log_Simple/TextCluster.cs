using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class TextCluster : MonoBehaviour
    {
        int _uid = 0;
        public string title;
        public List<string> LineList;

        public TextCluster(string title)
        {
            _uid = LogManager.Instance.GetNextID();
            this.title = $"{title}";
            LineList = new();
        }

        /// <summary>
        /// 이벤트 스크립트 중 - 캐릭터 대사와 나레이션용
        /// </summary>
        public void AddLine(eLineType _eLineType, EventScript _eventScript)
        {
            string charName, line, str = null ;
            if (_eLineType == eLineType.SPEAK)
            {
                charName = Util.GetCharNameKor(_eventScript.Character);
                line = _eventScript.Line;
                str = $"{charName} : {line}";
            }
            else if (_eLineType == eLineType.NARRATION)
            {
                line = _eventScript.Line;
                str = $"{line}";
            }
            LineList.Add(str);
        }
        /// <summary>
        /// 이벤트 - 결과 & 선택지 / 활동 중 캐릭터 대사 & 결과 
        /// </summary>
        public void AddLine(eLineType _eLineType, string _line)
        {
            string charName, str = null;
            // 이벤트&활동의 결과
            if (_eLineType == eLineType.RESULT)
            {
                str = $"{_line}";
            }
            // 활동의 캐릭터 대사
            else if (_eLineType == eLineType.SPEAK)
            {
                charName = Util.GetCharNameKor(DataManager.Instance.playerData.CharName);
                str = $"{charName} : {_line}";
            }
            else // NARRATION , 이벤트의 선택지
            {
                str = $"{_line}";
            }
            LineList.Add(str);

        }
    }

}
