using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleButtonControl : MonoBehaviour
{
    public void OnScheduleButtonClick() // 스케줄 창 띄우기 (팝업)
    {
        GameObject.Find("MainGameCanvas").transform.Find("ScheduleCanvas").gameObject.SetActive(true);
    }
    public void CloseSchedule() // 스케줄 창 끄기
    {
        GameObject.Find("MainGameCanvas").transform.Find("ScheduleCanvas").gameObject.SetActive(false);
    }
}
