using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    public override void Init()
    {
        UI_Manager.Instance.SetCanavas(gameObject, true);
    }

    public virtual void ClosePopupUI()
    {
        UI_Manager.Instance.ClosePopupUI(this);
    }

    public void CloseBTN()
    {
        //GameManager.Sound.Play(Define.Sound.SmallBTN);
        UI_Manager.Instance.ClosePopupUI();
    }
}