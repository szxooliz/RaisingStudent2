using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;
using UnityEngine.EventSystems;
using TMPro;
using System.Text;
using UnityEngine.UI;

namespace Client
{
    public class UI_LogPopup_Simple : UI_Popup
    {
        [SerializeField] GameObject Panel;
        [SerializeField] GameObject BTN_Close;

        [SerializeField] GameObject textClusterUI;
        [SerializeField] RectTransform parent;
        [SerializeField] ScrollRect scrollRect;

        List<GameObject> clusterUIList = new();
        // 클러스터별 라인 출력 상태를 저장하는 딕셔너리
        private Dictionary<int, int> clusterLineProgress = new();


        public override void Init()
        {
            base.Init();
            BindButton();
            StartCoroutine(ScrollToBottomDelayed());      
        }

        void BindButton()
        {
            BindEvent(Panel, OnClickPanel);
            BindEvent(BTN_Close, OnClickCloseBtn);
        }

        void OnClickCloseBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            gameObject.SetActive(false);
        }
        void OnClickPanel(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            InitLog();
        }

        private void InitLog()
        {
            List<TextCluster> clusters = LogManager.Instance.textClusters;

            for (int i = 0; i < clusters.Count; i++)
            {
                TextCluster cluster = clusters[i];
                int totalLines = cluster.LineList.Count;

                if (i < clusterUIList.Count)
                {
                    // 이미 UI에 있는 클러스터: 줄만 이어서 추가
                    int renderedLines = clusterLineProgress.ContainsKey(i) ? clusterLineProgress[i] : 0;

                    if (renderedLines < totalLines)
                    {
                        StringBuilder sb = new();
                        for (int j = renderedLines; j < totalLines; j++)
                        {
                            sb.AppendLine(cluster.LineList[j]);
                        }

                        var textComp = clusterUIList[i].GetComponent<TextMeshProUGUI>();
                        textComp.richText = true;
                        textComp.text += sb.ToString();

                        clusterLineProgress[i] = totalLines;
                    }
                }
                else
                {
                    // 새 클러스터 UI 생성
                    GameObject gm = Instantiate(textClusterUI, parent);
                    gm.GetComponent<TextMeshProUGUI>().richText = true;

                    StringBuilder sb = new();
                    sb.AppendLine($"<size=120%><b>[{cluster.title}]</b></size>");

                    foreach (string line in cluster.LineList)
                    {
                        sb.AppendLine(line);
                    }

                    gm.GetComponent<TextMeshProUGUI>().text = sb.ToString();

                    clusterUIList.Add(gm);
                    clusterLineProgress[i] = totalLines;
                }
            }
        }

        IEnumerator ScrollToBottomDelayed()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

}
