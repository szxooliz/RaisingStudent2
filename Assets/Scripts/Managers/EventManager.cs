using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    EventManager() { }

    // 활동 이후 이벤트 여부 체크
    public void CheckEvent()
    {
        Debug.Log("이벤트 체크 미구현");
        GameManager.Data.playerData.currentStatus = Define.Status.Main;
    }
    // 6. 이벤트 보여주기 ShowEvent 
    public void ShowEvent()
    {
        Debug.Log("이벤트 보여주기 미구현");
    }
}
