using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Client
{
    public class UI_EndingListScene : UI_Scene
    {
        [SerializeField] private GameObject endingItemPrefab; // Prefab 연결
        [SerializeField] private Transform contentParent;     // 생성된 Prefab의 부모 객체
        [SerializeField] private List<EndingIcon> endingIcons = new(); // 생성된 엔딩 아이콘 리스트
        [SerializeField] private Button BTN_Back;
 
        enum Buttons
        {
            BTN_Back
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));

            endingItemPrefab = Resources.Load<GameObject>("Prefabs/EndingItem");
            contentParent = GameObject.Find("EndingItems").transform;
            BindEvent(GetButton((int)Buttons.BTN_Back).gameObject, OnClickBack);

            PopulateEndingList();
        }

        /// <summary>
        /// 엔딩 리스트 화면 구성
        /// </summary>
        void PopulateEndingList()
        {
            List<eEndingName> endingNameList = Enum.GetValues(typeof(eEndingName))
               .Cast<eEndingName>()
               .Where(e => e != eEndingName.MaxCount)
               .ToList();


            // DataManager에서 엔딩 리스트 가져오기
            foreach (eEndingName endingName in endingNameList)
            {
                // Prefab 인스턴스 생성
                GameObject prefab = Instantiate(endingItemPrefab, contentParent);
                EndingIcon endingIcon = prefab.GetComponent<EndingIcon>();

                endingIcon.SetEndingNumber(endingName);
                endingIcon.SetIcon();
                endingIcons.Add(endingIcon);
            }
        }

        #region 버튼 이벤트
        void OnClickBack(PointerEventData evt)
        {
            SceneManager.LoadScene("TitleScene");
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
