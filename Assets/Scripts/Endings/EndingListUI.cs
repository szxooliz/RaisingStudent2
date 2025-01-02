using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingListUI : MonoBehaviour
{
    public GameObject endingItemPrefab; // 엔딩 항목 Prefab
    public Transform contentParent;     // Scroll View Content
    public GameObject popupEndingLocked;   // 해금되지 않은 팝업
    public GameObject popupEndingUnlocked; // 해금된 팝업

    // 팝업 UI 요소
    public TextMeshProUGUI unlockedTitleText;
    public TextMeshProUGUI unlockedResultText;

    private void Start()
    {
        PopulateEndingList();
    }

    private void PopulateEndingList()
    {
        foreach (var ending in EndingManager.Instance.endings)
        {
            GameObject endingItem = Instantiate(endingItemPrefab);
            endingItem.transform.SetParent(contentParent, false);

            // 이름 설정
            endingItem.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Ending" + ending.endingName;

            // 버튼 이벤트 연결
            Button button = endingItem.transform.Find("Button").GetComponent<Button>();
            button.onClick.AddListener(() => OnClickEndingItem(ending));

            Image lockIcon = endingItem.transform.Find("LockIcon").GetComponent<Image>();

            if (ending.isUnlocked)
            {
                lockIcon.enabled = false; // 잠금 해제 상태에서는 자물쇠 숨김
            }
            else
            {
                lockIcon.enabled = true; // 잠긴 상태에서는 자물쇠 표시
            }
        }
    }

    private void OnClickEndingItem(EndingData ending)
    {
        if (!ending.isUnlocked)
        {
            ShowLockedPopup();
        }
        else
        {
            ShowUnlockedPopup(ending);
        }
    }

    private void ShowLockedPopup()
    {
        popupEndingLocked.SetActive(true);
    }

    private void ShowUnlockedPopup(EndingData ending)
    {
        popupEndingUnlocked.SetActive(true);
        unlockedTitleText.text = "Ending" + ending.endingName;
        unlockedResultText.text = "Application field: " + ending.applicationField + "\nGrades: " + ending.grade + "\nAwards: " + ending.awards;
    }
}

