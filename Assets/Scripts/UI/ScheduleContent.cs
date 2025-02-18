using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class ScheduleContent : UI_Base
    {
        enum UIs
        {
            UI_Line, UI_Circle
        }

        public int EventIndex { get; private set; }
        
        public override void Init()
        {
            Bind<GameObject>(typeof(UIs));

            GetGameObject((int)UIs.UI_Line).SetActive(false);
            GetGameObject((int)UIs.UI_Circle).SetActive(false);
        }

        public void Initialize(int eventIndex)
        {
            EventIndex = eventIndex;
        }

        public void ToggleLine(bool isActive)
        {
            GetGameObject((int)UIs.UI_Line).SetActive(isActive);
        }

        public void ToggleCircle(bool isActive)
        {
            GetGameObject((int)UIs.UI_Circle).SetActive(isActive);
        }

    }
}
