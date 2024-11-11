using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonControl : MonoBehaviour
{
    public void OnScheduleButtonClick()
    {
        GameObject.Find("MainGameCanvas").transform.Find("ScheduleCanvas").gameObject.SetActive(true);
    }
    public void CloseSchedule()
    {
        GameObject.Find("MainGameCanvas").transform.Find("ScheduleCanvas").gameObject.SetActive(false);
    }
}
