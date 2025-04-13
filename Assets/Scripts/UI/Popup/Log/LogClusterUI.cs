using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class LogClusterUI : MonoBehaviour
    {
        [SerializeField] GameObject TitleObj; // TODO : 임시로 토글 버튼 대용
        [SerializeField] TextMeshProUGUI TMP_Title;
        [SerializeField] RectTransform rectTransform;
        List<GameObject> bubbleList;

        // 종류별 프리팹
        [SerializeField] GameObject yellowTailBubble;
        [SerializeField] GameObject yellowBubble;
        [SerializeField] GameObject greyBubble;

        public void InitClusterUI(LogCluster logCluster)
        {
            TMP_Title.text = logCluster.title;
            foreach(UnitLog unitLog in logCluster.unitLogs)
            {
                if (unitLog.eLineType == eLineType.SPEAK)
                {
                    GameObject gm = Instantiate(yellowTailBubble, rectTransform);
                    gm.GetComponent<YellowTailBubble>().InitBubbleUI(unitLog);
                    bubbleList.Add(gm);
                }
                else if (unitLog.eLineType == eLineType.NARRATION)
                {
                    GameObject gm = Instantiate(yellowBubble, rectTransform);
                    gm.GetComponent<Bubble>().InitBubbleUI(unitLog);
                    bubbleList.Add(gm);
                }
                else
                {
                    GameObject gm = Instantiate(greyBubble, rectTransform);
                    gm.GetComponent<Bubble>().InitBubbleUI(unitLog);
                    bubbleList.Add(gm);
                }
            }
        }
    }
}
