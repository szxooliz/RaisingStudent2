using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class UI_Manager : Singleton<UI_Manager>
    {
        /// <summary>
        /// 팝업 관리용 stack
        /// </summary>
        [Header("Pop Up")]
        Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

        int _order = 1;

        /// <summary>
        /// 팝업 재사용을 위한 캐싱
        /// </summary>
        Dictionary<System.Type, GameObject> _popupInstances = new Dictionary<System.Type, GameObject>();


        UI_Manager() { }

        /// <summary>
        /// UI의 부모
        /// </summary>
        [Header("Root")]
        GameObject _root = null;
        public GameObject Root
        {
            get
            {
                _root = GameObject.Find("@UI_Root");
                if (_root == null)
                    _root = new GameObject { name = "@UI_Root" };

                return _root;
            }
        }

        /// <summary>
        /// canvas 세팅
        /// </summary>
        /// <param name="go"></param>
        /// <param name="sort">팝업 정렬 방식 true: 차곡차곡</param>
        /// <param name="order">원하는 방식대로 팝업 놓을 때 순서</param>
        public void SetCanavas(GameObject go, bool sort = true, int order = 0)
        {
            Canvas canvas = Util.GetOrAddComponent<Canvas>(go);

            canvas.renderMode= RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;

            if (sort)
                canvas.sortingOrder = _order++;
            else
                canvas.sortingOrder = order;
        }

        /// <summary>
        /// UI_Scene 상속받는 클래스의 UI 프리팹 중 Scene에 속하는 것 Instantiate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T ShowSceneUI<T>(string name = null) where T : UI_Scene
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject go = ResourceManager.Instance.Instantiate($"UI/Scene/{name}");
            T sceneUI = Util.GetOrAddComponent<T>(go);

            go.transform.SetParent(Root.transform);

            return sceneUI;
        }

        /// <summary>
        /// UI_Popup 상속받는 클래스의 UI 프리팹 띄워줌
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T ShowPopupUI<T>(string name = null) where T : UI_Popup
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject popup;
            T popupUI;

            if(_popupInstances.TryGetValue(typeof(T), out popup) == false)
            {
                popup = ResourceManager.Instance.Instantiate($"UI/Popup/{name}");
                _popupInstances.Add(typeof(T), popup);

                popupUI = Util.GetOrAddComponent<T>(popup);
            }
            else
            {
                popupUI = Util.GetOrAddComponent<T>(popup);
                popupUI.ReOpenPopupUI();
                popupUI.GetComponent<Canvas>().sortingOrder = _order++;
            }

            _popupStack.Push(popupUI);

            popup.transform.SetParent(Root.transform);
            popup.SetActive(true);

            return popupUI;
        }

        public void ClosePopupUI()
        {
            if (_popupStack.Count <= 0) return;

            UI_Popup popup = _popupStack.Pop();
            popup.gameObject.SetActive(false);

            _order--;
        }

        /// <summary>
        /// 모든 pop up UI 닫기
        /// </summary>
        public void CloseAllPopupUI()
        {
            while (_popupStack.Count > 0)
                ClosePopupUI();
        }

        /// <summary>
        /// UI 초기화
        /// </summary>
        public void Clear()
        {
            CloseAllPopupUI();
            _popupInstances.Clear();
        }
    }
}
