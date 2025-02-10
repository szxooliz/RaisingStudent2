using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : Singleton<EventManager>
{
    #region Singleton
    EventManager() { }
    #endregion

    public int currentEventID = 0;
    private Dictionary<int, Action> eventActions;

    /// <summary>
    /// 활동 이후 이벤트 여부 체크
    /// </summary>
    public void CheckEvent()
    {
        switch (DataManager.Instance.playerData.currentTurn)
        {
            case 0: // 개강 - 현재 씬에서만 테스트용
                // currentEventID = (int)Define.Instance.MainEventName.Intro;
                // ShowEvent(currentEventID);
                Debug.Log((int)Define.MainEvents.Intro);
                break;
            case 1: // 테스트용
                currentEventID = (int)Define.MainEvents.Intro;
                ShowEvent();
                break;
            case 2: // 해커톤
                currentEventID = (int)Define.MainEvents.Hackerton;
                ShowEvent();
                break;
            case 5: // 중간고사
                currentEventID = (int)Define.MainEvents.MidTest_1;
                ShowEvent();
                break;
            case 6: // 체육대회
                currentEventID = (int)Define.MainEvents.SportsDay;
                ShowEvent();
                break;
            case 11: // 기말고사
                currentEventID = (int)Define.MainEvents.FinTest_1;
                ShowEvent();
                break;
            case 13: // 축제
                currentEventID = (int)Define.MainEvents.Festival;
                ShowEvent();
                break;
            case 17: // 중간고사
                currentEventID = (int)Define.MainEvents.MidTest_2;
                ShowEvent();
                break;
            case 19: // 지스타
                currentEventID = (int)Define.MainEvents.Gstar;
                ShowEvent();
                break;
            case 23: // 기말고사
                currentEventID = (int)Define.MainEvents.FinTest_2;
                ShowEvent();
                break;
            default:
                // 해당 없을 시 다시 메인 화면으로 전환
                DataManager.Instance.playerData.currentStatus = Define.Status.Main;
                break;
        }

    }
    
    /// <summary>
    /// 이벤트 실행
    /// </summary>
    public void ShowEvent()
    {
        Debug.Log("ShowEvent");
        DataManager.Instance.playerData.currentStatus = Define.Status.Event;
    }
}
