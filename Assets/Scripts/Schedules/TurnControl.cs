using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnControl : MonoBehaviour
{
    protected int[] Status = { 0, 0, 0, 0, 0 };
    protected int turn = 0;
    protected int StressGauge = (int)Define.ActivityType.Rest;
    private TMPro.TextMeshPro ScheduleMonth = GameObject.Find("MainGameCanvas").transform.Find("ScheduleUI").transform.Find("MonthText").gameObject.GetComponent<TMPro.TextMeshPro>();
    private TMPro.TextMeshPro ScheduleSeg = GameObject.Find("MainGameCanvas").transform.Find("ScheduleUI").transform.Find("SegText").gameObject.GetComponent<TMPro.TextMeshPro>();
    private void Awake() // 게임 시작할 때 UI를 업데이트하고 시작한다
    {
        UpdateScheduleUI(turn);
    }
    protected void OnStatusButtonClick()  // 스탯 올리는 버튼 눌렀을 때 기능
    {
        StatusUpdate(); // 스탯 업데이트해주고
        ChangeTurn(); // 다음턴으로 넘긴다
    }
    private void ChangeTurn() // 턴을 바꿔준다
    {
        turn++; // 다음 턴으로 넘어갔다는것을 표시한다
        Debug.Log($"현재 턴 : {turn}"); // 체크용
        if (turn > 23) EndGame(); // 24턴이 넘어가면 게임을 끝낸다
        UpdateScheduleUI(turn); // 턴을 인자로 받아 스케줄 UI를 업데이트해준다
    }
    private void UpdateScheduleUI(int turn)
    {
        switch (turn/3)
        {
            case 0:
                ScheduleMonth.text = "March";
                break;
            case 1:
                ScheduleMonth.text = "April";
                break;
            case 2:
                ScheduleMonth.text = "May";
                break;
            case 3:
                ScheduleMonth.text = "June";
                break;
            case 4:
                ScheduleMonth.text = "September";
                break;
            case 5:
                ScheduleMonth.text = "October";
                break;
            case 6:
                ScheduleMonth.text = "November";
                break;
            case 7:
                ScheduleMonth.text = "December";
                break;
            default:
                break;
        }
        switch (turn % 3)
        {
            case 0:
                ScheduleSeg.text = "First";
                break;
            case 1:
                ScheduleSeg.text = "Second";
                break;
            case 2:
                ScheduleSeg.text = "Third";
                break;
            default:
                break;
        }
        
    }
    private void EndGame() 
    {
        // 게임 종료 - 아직 미구현
    }
    protected virtual void StatusUpdate() 
    {
        // 스탯 업데이트 - 스탯별로 상속받아서 다른 스탯을 업데이트해준다 - 아직 미구현
    }
    protected void UpdateStatusUI()
    {
        // UI에서 스탯 업데이트 - 아직 미구현
    }
}
