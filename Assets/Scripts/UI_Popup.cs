using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    public override void Init()
    {
        GameManager.UI_Manager.SetCanavas(gameObject, true);
    }

    public virtual void ClosePopupUI()
    {
        GameManager.UI_Manager.ClosePopupUI(this);
    }

    public void CloseBTN()
    {
        //GameManager.Sound.Play(Define.Sound.SmallBTN);
        GameManager.UI_Manager.ClosePopupUI();
    }
}