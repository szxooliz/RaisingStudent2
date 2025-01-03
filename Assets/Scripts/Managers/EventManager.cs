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
        GameManager.Data.playerData.currentStatus = Define.Status.Main;
    }

    /// <summary>
    /// 이벤트 실행
    /// </summary>
    public void ShowEvent()
    {

    }
}
