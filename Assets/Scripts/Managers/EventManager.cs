using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    EventManager() { }


    // TODO : 세부 구현 필요

    /// <summary>
    /// 활동 이후 이벤트 여부 체크
    /// </summary>
    public void CheckEvent()
    {
        switch(DataManager.Instance.playerData.currentTurn)
        {
            case 0: // 개강
                Debug.Log("CheckEvent");
                ShowEvent();
                break;
            case 2: // 해커톤
                break;
            case 5: // 중간고사
                break;
            case 6: // 체육대회
                break;
            case 11: // 기말고사
                break;
            case 13: // 축제
                break;
            case 17: // 중간고사
                break;
            case 19: // 지스타
                break;
            case 23: // 기말고사
                break;
        }
        DataManager.Instance.playerData.currentStatus = Define.Status.Main;
    }

    /// <summary>
    /// 이벤트 실행
    /// </summary>
    public void ShowEvent()
    {
        DataManager.Instance.playerData.currentStatus = Define.Status.Event;
        Debug.Log("ShowEvent");

        EventUI eventUI = GameObject.FindObjectOfType<EventUI>(true);

        if(eventUI != null)
        {
            eventUI.StartDialogue();
        }
        else{
            Debug.LogError("EventUI를 찾을 수 없습니다.");
        }
    }
}
