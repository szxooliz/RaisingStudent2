using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class UI_Manager : Singleton<UI_Manager>
    {
        int _order = 0;

        Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
        UI_Scene _scene = null;

        UI_Manager() { }

        public GameObject Root
        {
            get
            {
                GameObject root = GameObject.Find("@UI_Root");
                if (root == null)
                    root = new GameObject { name = "@UI_Root" };

                return root;
            }
        }

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

        //public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
        //{
        //    if (string.IsNullOrEmpty(name))
        //        name = typeof(T).Name;



        //    GameObject go = GameManager.Resource.Instantiate($"UI/SubItem/{name}");

        //    if (parent != null)
        //        go.transform.SetParent(parent);

        //    return Util.GetOrAddComponent<T>(go);
        //}


        public T ShowSceneUI<T>(string name = null) where T : UI_Scene
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject go = ResourceManager.Instance.Instantiate($"UI/Scene/{name}");
            T sceneUI = Util.GetOrAddComponent<T>(go);
            _scene = sceneUI;

            go.transform.SetParent(Root.transform);

            return sceneUI;
        }

        public T ShowPopupUI<T>(string name = null) where T : UI_Popup
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject go = ResourceManager.Instance.Instantiate($"UI/Popup/{name}");
            T popup = Util.GetOrAddComponent<T>(go);
            _popupStack.Push(popup);

            go.transform.SetParent(Root.transform);

            return popup;
        }


        public void ClosePopupUI(UI_Popup popup)
        {
            if (_popupStack.Count == 0)
                return;

            if (_popupStack.Peek() != popup)
            {
                Debug.Log("Close Popup Failed");
                return;
            }

            ClosePopupUI();
        }

        public void ClosePopupUI()
        {
            if (_popupStack.Count == 0)
                return;

            UI_Popup popup = _popupStack.Pop();
            ResourceManager.Instance.Destroy(popup.gameObject);
            popup = null;

            _order--;
        }

        public void CloseALlPopupUI()
        {
            while (_popupStack.Count > 0)
                ClosePopupUI();
        }

        public void Clear()
        {
            CloseALlPopupUI();
        }
    }
}
