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
        switch(DataManager.Instance.playerData.currentTurn)
        {
            case 0: // 개강 - 현재 씬에서만 테스트용
                // currentEventID = (int)Define.Instance.MainEventName.Intro;
                // ShowEvent(currentEventID);
                Debug.Log((int)Define.MainEventName.Intro);
                break;
            case 1: // 테스트용
                currentEventID = (int)Define.MainEventName.Intro;
                ShowEvent();
                break;
            case 2: // 해커톤
                currentEventID = (int)Define.MainEventName.Hackerton;
                ShowEvent();
                break;
            case 5: // 중간고사
                currentEventID = (int)Define.MainEventName.MidTest_1;
                ShowEvent();
                break;
            case 6: // 체육대회
                currentEventID = (int)Define.MainEventName.SportsDay;
                ShowEvent();
                break;
            case 11: // 기말고사
                currentEventID = (int)Define.MainEventName.FinTest_1;
                ShowEvent();
                break;
            case 13: // 축제
                currentEventID = (int)Define.MainEventName.Festival;
                ShowEvent();
                break;
            case 17: // 중간고사
                currentEventID = (int)Define.MainEventName.MidTest_2;
                ShowEvent();
                break;
            case 19: // 지스타
                currentEventID = (int)Define.MainEventName.Gstar;
                ShowEvent();
                break;
            case 23: // 기말고사
                currentEventID = (int)Define.MainEventName.FinTest_2;
                ShowEvent();
                break;
            default:
                // 해당 없을 시 다시 메인 화면으로 전환
                DataManager.Instance.playerData.currentStatus = Define.Status.Main;
                break;
        }

        /* // 이벤트별 작업을 매핑
        eventActions = new Dictionary<int, Action>
        {
            { 0, () => { currentEventID = (int)Define.MainEventName.Intro; ShowEvent(); } }, // 개강
            { 1, () => { Debug.Log((int)Define.MainEventName.Intro); currentEventID = (int)Define.MainEventName.Intro; ShowEvent(); } }, // 테스트용
            { 2, () => { currentEventID = (int)Define.MainEventName.Hackerton; ShowEvent(); } }, // 해커톤
            { 5, () => { currentEventID = (int)Define.MainEventName.MidTest_1; ShowEvent(); } }, // 중간고사
            { 6, () => { currentEventID = (int)Define.MainEventName.SportsDay; ShowEvent(); } }, // 체육대회
            { 11, () => { currentEventID = (int)Define.MainEventName.FinTest_1; ShowEvent(); } }, // 기말고사
            { 13, () => { currentEventID = (int)Define.MainEventName.Festival; ShowEvent(); } }, // 축제
            { 17, () => { currentEventID = (int)Define.MainEventName.MidTest_2; ShowEvent(); } }, // 중간고사
            { 19, () => { currentEventID = (int)Define.MainEventName.Gstar; ShowEvent(); } }, // 지스타
            { 23, () => { currentEventID = (int)Define.MainEventName.FinTest_2; ShowEvent(); } }, // 기말고사
        };

        // currentTurn에 맞는 작업 실행
        if (eventActions.ContainsKey(DataManager.Instance.playerData.currentTurn))
        {
            eventActions[DataManager.Instance.playerData.currentTurn].Invoke();
        }
        else
        {
            // 해당 없을 시 메인 화면으로 전환
            DataManager.Instance.playerData.currentStatus = Define.Status.Main;
        } */
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
