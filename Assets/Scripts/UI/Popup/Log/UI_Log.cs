using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    /// <summary>
    /// 한 프로세스 로그 블록 단위
    /// </summary>
    public class UI_Log : UI_Base
    {
        #region enum
        enum Texts
        {
            TMP_Title
        }
        enum Buttons
        {
            BTN_Toggle
        }
        #endregion

        // 로그 박스 하나당 프로세스 데이터 하나 매핑
        // 이건 어디서 해줘야 하지?
        // -> 활동 or 이벤트 처음 시작할 때 생성 밎 매핑

        // 이벤트 끝나고 봤던 이벤트로 기록하는 시점에서 박스 하나 통째로 저장 > 가능?
        
        // 대사(말풍선) 개수 유동적임
        // 박스 배경 길이도 유동적


    } 

}