using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BaseSceneTxt : UI_Scene
{
    /*
    필요한 것 정리
    1. 스탯 이름, 스탯 수치
    2. 스트레스 수치
    3. 활동 버튼들 -> 누르면 활동 이벤트 발생
    4. 턴 기록
    5. 스케줄 확인 버튼 -> 팝업 O
    6. 로그 버튼 -> 팝업 O
    7. 현재 상황 표시: 로그버튼 옆에있던거
    8. 메뉴 버튼 -> 팝업 O

    참고: 대화 말풍선은 팝업으로

    버튼 / 텍스트 UI 분리해서 스크립트 만듦
    버튼 누르고 이벤트 발생하면 액션사용해서 텍스트 쪽으로 변경사항 적용되도록
    */
    enum Texts
    {
        // 스탯 이름들
        InteliNameTMP, OtakuNameTMP, StrengthNameTMP, CharmingNameTMP,
        // 스탯 수치들
        InteliTMP, OtakuTMP, StrengthTMP, CharmingTMP, 

        MaxCount
    }

    public override void Init()
    {
        base.Init();
        Bind<TMPro.TMP_Text>(typeof(Texts));
    }
}
