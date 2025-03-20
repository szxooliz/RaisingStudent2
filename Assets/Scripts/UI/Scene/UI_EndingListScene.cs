using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class UI_EndingListScene : UI_Scene
    {
        [SerializeField] private GameObject endingItemPrefab; // Prefab 연결
        [SerializeField] private Transform contentParent;     // 생성된 Prefab의 부모 객체

        private string spritePath = "Sprites/UI/Ending/";

        enum Buttons
        {
            BTN_Image,
        }

        enum Images
        {
            IMG_LockIcon,
        }

        enum Texts
        {
            TMP_Name,
        }

        public override void Init()
        {
            base.Init();

            endingItemPrefab = Resources.Load<GameObject>("Prefabs/EndingItem");
            contentParent = GameObject.Find("EndingItems").transform;

            PopulateEndingList();
        }

        /// <summary>
        /// 엔딩 리스트 화면 구성
        /// </summary>
        void PopulateEndingList()
        {
            // DataManager에서 엔딩 리스트 가져오기
            foreach (var ending in DataManager.Instance.persistentData.endingList)
            {
                if (ending.endingName == eEndingName.MaxCount)
                {
                    return;
                }

                // Prefab 인스턴스 생성
                GameObject endingItem = Instantiate(endingItemPrefab, contentParent);

                // UI 요소 바인딩
                var buttons = Bind<Button>(endingItem, typeof(Buttons));
                var images = Bind<Image>(endingItem, typeof(Images));
                var texts = Bind<TMPro.TMP_Text>(endingItem, typeof(Texts));

                int index = (int)(ending.endingName);

                // 텍스트, 버튼 및 자물쇠 아이콘 설정
                if (ending.isUnlocked)
                {
                    string imagePath = spritePath + (char)('A' + index) + "_off";
                    buttons[(int)Buttons.BTN_Image].image.sprite = DataManager.Instance.GetOrLoadSprite(imagePath);
                    texts[(int)Texts.TMP_Name].text = GetEndingNameKor(ending.endingName);
                    BindEvent(buttons[(int)Buttons.BTN_Image].gameObject, (PointerEventData evt) => onClickUnlockedEnding(ending));
                    images[(int)Images.IMG_LockIcon].gameObject.SetActive(false);
                }
                else
                {
                    string imagePath = spritePath + (char)('A' + index) + "_lock";
                    buttons[(int)Buttons.BTN_Image].image.sprite = DataManager.Instance.GetOrLoadSprite(imagePath);

                    texts[(int)Texts.TMP_Name].text = "엔딩" + (char)('A' + index);
                    BindEvent(buttons[(int)Buttons.BTN_Image].gameObject, (PointerEventData evt) => onClickLockedEnding(ending));
                    BindEvent(images[(int)Images.IMG_LockIcon].gameObject, (PointerEventData evt) => onClickLockedEnding(ending));
                    images[(int)Images.IMG_LockIcon].gameObject.SetActive(true);
                }
            }
        }

        #region 버튼 이벤트
        public void onClickUnlockedEnding(Ending ending)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);
            var popup = UI_Manager.Instance.ShowPopupUI<UI_UnlockedEndingPopup>();
            popup.SetUnlockedEndingPopup(ending);
        }

        public void onClickLockedEnding(Ending ending)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);
            var popup = UI_Manager.Instance.ShowPopupUI<UI_LockedEndingPopup>();
            popup.SetLockedEndingPopup(ending);
        }
        #endregion

        #region Prefab 바인딩 메서드
        private Dictionary<int, T> Bind<T>(GameObject prefab, System.Type enumType) where T : UnityEngine.Object
        {
            Dictionary<int, T> bindings = new Dictionary<int, T>();
            foreach (int value in System.Enum.GetValues(enumType))
            {
                string name = System.Enum.GetName(enumType, value);
                Transform child = prefab.transform.Find(name);
                if (child != null)
                {
                    T component = child.GetComponent<T>();
                    if (component != null)
                        bindings.Add(value, component);
                }
            }
            return bindings;
        }
        #endregion
    }
}
