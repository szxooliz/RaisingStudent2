using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;
using System;
using System.Linq;

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
            List<eEndingName> endingNameList = new List<eEndingName>((eEndingName[])Enum.GetValues(typeof(eEndingName)));

            var endingDict = DataManager.Instance.persistentData.endingList
                .ToDictionary(e => e.endingName, e => e);

            // DataManager에서 엔딩 리스트 가져오기
            foreach (eEndingName endingName in endingNameList)
            {
                // Prefab 인스턴스 생성
                GameObject endingItem = Instantiate(endingItemPrefab, contentParent);

                // UI 요소 바인딩
                var buttons = Bind<Button>(endingItem, typeof(Buttons));
                var images = Bind<Image>(endingItem, typeof(Images));
                var texts = Bind<TMPro.TMP_Text>(endingItem, typeof(Texts));

                // 엔딩을 나타내는 알파벳
                char endingAlpha = (char)('A' + (int)(endingName));

                // 해금 여부를 기반으로 텍스트, 버튼 및 자물쇠 아이콘 설정
                if (endingDict.ContainsKey(endingName))
                {
                    string imagePath = spritePath + endingAlpha + "_off";
                    buttons[(int)Buttons.BTN_Image].image.sprite = DataManager.Instance.GetOrLoadSprite(imagePath);
                    texts[(int)Texts.TMP_Name].text = GetEndingNameKor(endingName);
                    BindEvent(buttons[(int)Buttons.BTN_Image].gameObject, (PointerEventData evt) => onClickUnlockedEnding(endingDict[endingName]));
                    images[(int)Images.IMG_LockIcon].gameObject.SetActive(false);
                }
                else
                {
                    string imagePath = spritePath + endingAlpha + "_lock";
                    buttons[(int)Buttons.BTN_Image].image.sprite = DataManager.Instance.GetOrLoadSprite(imagePath);

                    texts[(int)Texts.TMP_Name].text = "엔딩" + endingAlpha;
                    BindEvent(buttons[(int)Buttons.BTN_Image].gameObject, (PointerEventData evt) => onClickLockedEnding(endingName));
                    BindEvent(images[(int)Images.IMG_LockIcon].gameObject, (PointerEventData evt) => onClickLockedEnding(endingName));
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

        public void onClickLockedEnding(eEndingName endingName)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);
            var popup = UI_Manager.Instance.ShowPopupUI<UI_LockedEndingPopup>();
            popup.SetLockedEndingPopup(endingName);
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
