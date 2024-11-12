using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassStatusButton : TurnControl
{
    int type = (int) Define.ActivityType.Class;
    public void OnClassStatusButtonClick()
    {
        OnStatusButtonClick();
    }
    protected override void StatusUpdate() // 스탯 올려주기
    {
        base.StatusUpdate(); // 공통부분
        Status[type] += 10;
        Status[StressGauge] += 10;
        UpdateStatusUI();
    }
}
