using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnUIUpdater : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI ScheduleMonth;
    [SerializeField] private TMPro.TextMeshProUGUI ScheduleSeg;
    [SerializeField] private TMPro.TextMeshProUGUI RemainingTurn;
    private int turn = 0;
    private void Start() 
    {
        UpdateScheduleUI(turn++); // 게임 시작할 때 UI 업데이트
    }
    public void CallUpdateScheduleUI()
    {
        UpdateScheduleUI(turn++); // UI 업데이트 후 턴 + 1
        Debug.Log($"현재 턴 : {turn}"); // 체크용
        if (turn > 24) EndGame(); // 24턴이 넘어가면 게임을 끝낸다
    }
    private void EndGame() // 게임 종료
    {
        // 미구현
    }
    private void UpdateScheduleUI(int turn) // 스케줄 UI 업데이트
    {
        switch (turn / 3)
        {
            case 0:
                ScheduleMonth.SetText("March");
                break;
            case 1:
                ScheduleMonth.SetText("April");
                break;
            case 2:
                ScheduleMonth.SetText("May");
                break;
            case 3:
                ScheduleMonth.SetText("June");
                break;
            case 4:
                ScheduleMonth.SetText("September");
                break;
            case 5:
                ScheduleMonth.SetText("October");
                break;
            case 6:
                ScheduleMonth.SetText("November");
                break;
            case 7:
                ScheduleMonth.SetText("December");
                break;
            default:
                break;
        }
        switch (turn % 3)
        {
            case 0:
                ScheduleSeg.SetText("First");
                break;
            case 1:
                ScheduleSeg.SetText("Second");
                break;
            case 2:
                ScheduleSeg.SetText("Third");
                break;
            default:
                break;
        }
        RemainingTurn.SetText($"{turn+1} Turn");
    }
}
