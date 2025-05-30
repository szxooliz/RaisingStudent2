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

        [SerializeField] GameObject logClusterUI;
        [SerializeField] RectTransform parent;

        [SerializeField] GameObject Panel;
        [SerializeField] GameObject BTN_Close;


        int lastClusterID = 0;
        List<LogClusterUI> clusterUIList = new();

        public override void Init()
        {
            base.Init();
            BindButton();
        }
        private void OnEnable()
        {
            InitLogClusters();
        }

        void InitLogClusters()
        {
            // 이제 문제는 클러스터 자체가 두개 생기는 것
            // 클러스터 자체의 uid를 가지고 생겼던 것은 instanciate 패스!!
            // 마지막 것만 추가된거 초기화 위해 함수 호출하자...
            int currentClusterID = LogManager.Instance.logClusterList.Count;

            for (int i = lastClusterID; i < currentClusterID; i++)
            {
                LogCluster cluster = LogManager.Instance.logClusterList[i];
                GameObject gm = Instantiate(logClusterUI, parent);

                LogClusterUI clusUI = gm.GetComponent<LogClusterUI>();
                clusUI.InitClusterUI(cluster);
                clusterUIList.Add(clusUI);
            }
            // 제일 최근 클러스터 세팅
            clusterUIList[^1].InitClusterUI(LogManager.Instance.logClusterList[^1]);
            lastClusterID = currentClusterID;

            //foreach (LogCluster cluster in LogManager.Instance.logClusterList)
            //{
            //    GameObject gm = Instantiate(logClusterUI, parent);
            //    gm.GetComponent<LogClusterUI>().InitClusterUI(cluster);
            //}
        }

        void BindButton()
        {
            BindEvent(Panel, OnClickPanel);
            BindEvent(BTN_Close, OnClickCloseBtn);
        }

        void OnClickCloseBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            //ClosePopupUI();
            gameObject.SetActive(false);
        }
        void OnClickPanel(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            gameObject.SetActive(false);
        }
    }

}
