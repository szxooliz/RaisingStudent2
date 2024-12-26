using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class UI_Popup : UI_Base
    {
        public override void Init()
        {
            UI_Manager.Instance.SetCanavas(gameObject, true);
        }

        public virtual void ClosePopupUI()
        {
            UI_Manager.Instance.ClosePopupUI();
        }

        public virtual void ReOpenPopupUI() { }

    }
}
