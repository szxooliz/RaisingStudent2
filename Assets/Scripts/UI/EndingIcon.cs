using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class EndingIcon : MonoBehaviour
    {
        [SerializeField] Button pageButton;
        [SerializeField] Button lockButton;
        [SerializeField] TMPro.TextMeshProUGUI TMP_endingName;

        [SerializeField] Sprite onSprite;
        [SerializeField] Sprite offSprite;
        [SerializeField] Sprite lockSprite;

        private string spritePath = "Sprites/UI/Ending/";
        private eEndingName eEndingNumber;

        private void Start()
        {
            lockButton.onClick.AddListener(OnClickLockedEnding);
            pageButton.onClick.AddListener(OnClickUnlockedEnding);
        }

        public void SetEndingNumber(eEndingName eEndingName)
        {
            eEndingNumber = eEndingName;
            GetSprites();
        }

        public void GetSprites()
        {
            char endingCode = (char)('A' + (int)(eEndingNumber));

            string lockImagePath = $"{spritePath}{endingCode}_lock";
            string offImagePath = $"{spritePath}{endingCode}_off";
            string onImagePath = $"{spritePath}{endingCode}_on";

            lockSprite = DataManager.Instance.GetOrLoadSprite(lockImagePath);
            offSprite = DataManager.Instance.GetOrLoadSprite(offImagePath);
            onSprite = DataManager.Instance.GetOrLoadSprite(onImagePath);
        }

        public void SetIcon()
        {
            char endingCode = (char)('A' + (int)(eEndingNumber));
            bool isUnlocked = DataManager.Instance.persistentData.endingDict.ContainsKey(eEndingNumber);

            // 자물쇠 버튼 활성화 여부 & 페이지 이미지 설정
            lockButton.gameObject.SetActive(!isUnlocked);
            pageButton.image.sprite = isUnlocked ? offSprite : lockSprite;
            TMP_endingName.text = $"엔딩{endingCode}";

            SpriteState spriteState = pageButton.spriteState;
            spriteState.pressedSprite = onSprite;
        }

        /// <summary> 클릭 시 미해금 팝업 </summary>
        public void OnClickLockedEnding()
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);
            var popup = UI_Manager.Instance.ShowPopupUI<UI_LockedEndingPopup>();
            popup.SetLockedEndingPopup(eEndingNumber);
        }

        /// <summary> 클릭 시 엔딩 내용 팝업 </summary>
        public void OnClickUnlockedEnding()
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);

            Ending ending = DataManager.Instance.persistentData.endingDict[eEndingNumber];
            var popup = UI_Manager.Instance.ShowPopupUI<UI_UnlockedEndingPopup>();
            popup.SetUnlockedEndingPopup(ending);
        }

    }

}
