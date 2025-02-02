using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

namespace Client
{
    public class UI_LogPopup : UI_Popup
    {
        GameObject Content;
        GameObject logPrefab;
        // TODO : 활동, 이벤트 담은 프리팹 불러오기 + 캐시 사용
        enum Buttons
        {
            Panel,
            BTN_Close
        }
        enum UI_Log
        {
            Log_BTN, Log_TITLE, Log_BODY
        }
        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            BindButton();

            Content = gameObject.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).gameObject; // UI_LogPopup -> Scroll View -> Viewport -> Content 가리킴
            
            logPrefab = Resources.Load<GameObject>("Prefabs/UI/Popup/UI_Log"); // 프리팹 생성을 어떻게 해야 할지 몰라 일단 경로에서 불러옴

            if(logPrefab == null) Debug.LogError("UI_Log 프리팹 미존재");
        }
        private void OnEnable()
        {
            LogPopupUpdate();
        }
        private void Update()
        {
            ResizeLog();
        }
        /// <summary>
        /// 로그 팝업 업데이트
        /// </summary>
        void LogPopupUpdate()
        {
            int logNum = Content.transform.childCount;
            int listLength = DataManager.Instance.activityList.Count;

            // log 구성물 생성
            for (; logNum < listLength; logNum++)
            {
                Instantiate(logPrefab).transform.SetParent(Content.transform);
            }
            Debug.Log(logNum);
            Debug.Log(listLength);
            // 프리팹으로 구성물 추가 세부 내용 변경
            for (int i = 0; i < logNum; i++)
            {
                Content.transform.GetChild(i).transform.GetChild((int)UI_Log.Log_BODY).GetComponent<TMP_Text>().text // UI_Log 프리팹의 Log_BODY 부분
                    = DataManager.Instance.activityList[i].activityType.ToString(); // activityList에 string이 없어서 일단 활동 이름 string으로 변환
            }
        }
        void ResizeLog()
        {
            RectTransform logTransform = Content.transform.GetComponent<RectTransform>();
            RectTransform[] componentTransform = Content.transform.GetComponentsInChildren<RectTransform>();

            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            foreach (RectTransform child in componentTransform)
            {
                if (child == logTransform) continue;

                Vector3[] corners = new Vector3[4];
                child.GetWorldCorners(corners);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 localPoint = logTransform.InverseTransformPoint(corners[i]); // 로컬 좌표 변환
                    min = Vector2.Min(min, localPoint);
                    max = Vector2.Max(max, localPoint);
                }
            }

            Vector2 newSize = max - min;
            logTransform.sizeDelta = newSize;
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_Close).gameObject, OnClickCloseBtn);
        }

        void OnClickCloseBtn(PointerEventData evt)
        {
            ClosePopupUI();
        }
        void OnClickPanel(PointerEventData evt)
        {
            ClosePopupUI();
        }
    }

}
