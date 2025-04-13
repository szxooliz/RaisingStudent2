using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class UI_LogPopup : UI_Popup
    {
        enum Buttons
        {
            Panel, BTN_Close
        }

        [SerializeField] GameObject logClusterUI;
        [SerializeField] RectTransform parent;
        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            BindButton();
        }
        private void OnEnable()
        {
            //InitLogClusters();
        }

        void InitLogClusters()
        {
            foreach(LogCluster cluster in LogManager.Instance.logClusterList)
            {
                GameObject gm = Instantiate(logClusterUI, parent);
                gm.GetComponent<LogClusterUI>().InitClusterUI(cluster);
            }
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_Close).gameObject, OnClickCloseBtn);
        }

        void OnClickCloseBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        void OnClickPanel(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
    }

}
