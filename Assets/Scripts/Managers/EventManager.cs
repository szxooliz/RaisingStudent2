using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    #region Singleton
    EventManager() { }
    #endregion

    public long currentEventID = 0;

    /// <summary>
    /// 활동 이후 이벤트 여부 체크
    /// </summary>
    public void CheckEvent()
    {
        switch(DataManager.Instance.playerData.currentTurn)
        {
            case 1: // 개강 - 현재 씬에서만 테스트용
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
        DataManager.Instance.playerData.currentStatus = Define.Status.Event;
    }
}
