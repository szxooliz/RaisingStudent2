using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnUIUpdater : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI ScheduleMonth;
    [SerializeField] private TMPro.TextMeshProUGUI ScheduleSeg;
    [SerializeField] private TMPro.TextMeshProUGUI RemainingTurn;
    [SerializeField] private List<GameObject> Contents;
    [SerializeField] private Image Cursor;
    private int turn = 0;
    private int event_counter = 0;
    private void Start() 
    {
        UpdateScheduleUI(turn++); // 게임 시작할 때 UI 업데이트
    }
    public void CallUpdateScheduleUI()
    {
        UpdateScheduleUI(turn++); // UI 업데이트 후 턴 + 1
        Debug.Log($"현재 턴 : {turn}"); // 체크용
    }
    private void EndGame() // 게임 종료
    {
        SceneManager.LoadScene("EndingScene");
    }
    private void ContentsCursorController(int ContentNum)
    {
        Contents[ContentNum].SetActive(true);
        if (ContentNum < 6)
        {
            float temp = (float)(100-ContentNum*50);
            Cursor.rectTransform.anchoredPosition = new Vector2(-68, temp);
        }
    }
    private void UpdateScheduleUI(int turn) // 스케줄 UI 업데이트
    {
        switch (turn)
        {
            case 0:
                ScheduleMonth.SetText("March");
                ScheduleSeg.SetText("First");
                ContentsCursorController(event_counter++);
                break;
            case 1:
                ScheduleMonth.SetText("March");
                ScheduleSeg.SetText("Second");
                break;
            case 2:
                ScheduleMonth.SetText("March");
                ScheduleSeg.SetText("Third");
                break;
            case 3:
                ScheduleMonth.SetText("April");
                ScheduleSeg.SetText("First");
                break;
            case 4:
                ScheduleMonth.SetText("April");
                ScheduleSeg.SetText("Second");
                break;
            case 5:
                ScheduleMonth.SetText("April");
                ScheduleSeg.SetText("Third");
                break;
            case 6:
                ScheduleMonth.SetText("May");
                ScheduleSeg.SetText("First");
                ContentsCursorController(event_counter++);
                break;
            case 7:
                ScheduleMonth.SetText("May");
                ScheduleSeg.SetText("Second");
                break;
            case 8:
                ScheduleMonth.SetText("May");
                ScheduleSeg.SetText("Third");
                break;
            case 9:
                ScheduleMonth.SetText("June");
                ScheduleSeg.SetText("First");
                break;
            case 10:
                ScheduleMonth.SetText("June");
                ScheduleSeg.SetText("Second");
                break;
            case 11:
                ScheduleMonth.SetText("June");
                ScheduleSeg.SetText("Third");
                break;
            case 12:
                ScheduleMonth.SetText("September");
                ScheduleSeg.SetText("First");
                ContentsCursorController(event_counter++);
                break;
            case 13:
                ScheduleMonth.SetText("September");
                ScheduleSeg.SetText("Second");
                ContentsCursorController(event_counter++);
                break;
            case 14:
                ScheduleMonth.SetText("September");
                ScheduleSeg.SetText("Third");
                break;
            case 15:
                ScheduleMonth.SetText("October");
                ScheduleSeg.SetText("First");
                break;
            case 16:
                ScheduleMonth.SetText("October");
                ScheduleSeg.SetText("Second");
                break;
            case 17:
                ScheduleMonth.SetText("October");
                ScheduleSeg.SetText("Third");
                break;
            case 18:
                ScheduleMonth.SetText("November");
                ScheduleSeg.SetText("First");
                ContentsCursorController(event_counter++);
                break;
            case 19:
                ScheduleMonth.SetText("November");
                ScheduleSeg.SetText("Second");
                break;
            case 20:
                ScheduleMonth.SetText("November");
                ScheduleSeg.SetText("Third");
                break;
            case 21:
                ScheduleMonth.SetText("December");
                ScheduleSeg.SetText("First");
                break;
            case 22:
                ScheduleMonth.SetText("December");
                ScheduleSeg.SetText("Second");
                ContentsCursorController(event_counter++);
                break;
            case 23:
                ScheduleMonth.SetText("December");
                ScheduleSeg.SetText("Third");
                ContentsCursorController(event_counter++);
                break;
            default:
                Cursor.gameObject.SetActive(false);
                EndGame();
                break;
        }
        RemainingTurn.SetText($"{turn+1} Turn");
    }
}
