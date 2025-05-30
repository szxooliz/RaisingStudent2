using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class LogClusterUI : MonoBehaviour
    {
        [SerializeField] GameObject TitleObj; // TODO : 임시로 토글 버튼 대용
        [SerializeField] TextMeshProUGUI TMP_Title;
        [SerializeField] RectTransform rectTransform;
        [SerializeField] RectTransform contentRectTransform;
        
        List<GameObject> bubbleList = new();
        LogCluster logCluster;
        int lastLineCount = 0;
        bool isToggled = false;

        // 종류별 프리팹
        [SerializeField] GameObject yellowTailBubble;
        [SerializeField] GameObject yellowBubble;
        [SerializeField] GameObject greyBubble;
        private void Awake()
        {
            TitleObj.GetComponent<Button>().onClick.AddListener(Toggle);
        }
        public void InitClusterUI(LogCluster _logCluster)
        {
            // 정보 초기화
            this.logCluster = _logCluster;
            int currentLineCount = _logCluster.unitLogs.Count;

            // 제목 UI 텍스트 초기화
            TMP_Title.text = logCluster.title;


            // 새로 추가된 대사만 Instantiate
            for (int i = lastLineCount; i < currentLineCount; i++)
            {
                UnitLog unitLog = logCluster.unitLogs[i];

                if (unitLog.eLineType == eLineType.SPEAK)
                {
                    GameObject gm = Instantiate(yellowTailBubble, contentRectTransform);
                    gm.GetComponent<YellowTailBubble>().InitBubbleUI(unitLog);
                    bubbleList.Add(gm);
                }
                else if (unitLog.eLineType == eLineType.NARRATION)
                {
                    GameObject gm = Instantiate(yellowBubble, contentRectTransform);
                    gm.GetComponent<Bubble>().InitBubbleUI(unitLog);
                    bubbleList.Add(gm);
                }
                else
                {
                    GameObject gm = Instantiate(greyBubble, contentRectTransform);
                    gm.GetComponent<Bubble>().InitBubbleUI(unitLog);
                    bubbleList.Add(gm);
                }
            }
            ResizeToFitChildren(contentRectTransform.gameObject);
            ResizeToFitChildren(gameObject);
            lastLineCount = currentLineCount;

        }

        public void ResizeToFitChildren(GameObject gm)
        {
            RectTransform parentRect = gm.GetComponent<RectTransform>();
            if (parentRect == null || parentRect.childCount == 0)
                return;

            // 초기 경계를 첫 자식으로 설정
            RectTransform firstChild = parentRect.GetChild(0) as RectTransform;
            Vector3[] corners = new Vector3[4];
            firstChild.GetWorldCorners(corners);
            Vector3 min = corners[0];
            Vector3 max = corners[2];

            // 모든 자식의 경계를 포함하도록 min/max 계산
            for (int i = 1; i < parentRect.childCount; i++)
            {
                RectTransform child = parentRect.GetChild(i) as RectTransform;
                if (child == null) continue;

                child.GetWorldCorners(corners);
                min = Vector3.Min(min, corners[0]);
                max = Vector3.Max(max, corners[2]);
            }

            // 부모의 크기를 자식의 전체 크기로 조정
            Vector2 size = max - min;
            parentRect.sizeDelta = size;

            // 부모의 위치 보정 (Pivot을 고려해서)
            Vector3 worldCenter = (min + max) / 2f;
            Vector3 localCenter;
            RectTransform parent = parentRect.parent as RectTransform;
            if (parent != null)
            {
                localCenter = parent.InverseTransformPoint(worldCenter);
            }
            else
            {
                localCenter = worldCenter;
            }
            parentRect.localPosition = localCenter;
        }

        private void Toggle()
        {
            // TODO : 버튼 이미지 변경하는거 추가 필요
            isToggled = !isToggled;
            foreach(GameObject gm in bubbleList)
            {
                gm.SetActive(isToggled);
            }
            ResizeToFitChildren(contentRectTransform.gameObject);
        }
    }
}
