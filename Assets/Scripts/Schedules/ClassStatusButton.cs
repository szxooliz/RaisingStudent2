using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassStatusButton : MonoBehaviour
{
    int type = (int) Define.ActivityType.Class;
    public void CallStatusUpdate()
    {
        StatusUpdate();
    }
    private void StatusUpdate() // 스탯 올려주기
    {
        // 미구현
    }
}
