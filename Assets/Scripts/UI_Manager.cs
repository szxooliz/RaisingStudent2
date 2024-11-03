using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager
{
    int _order = 0;

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _scene = null;

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

    public void SetCanavas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        //canvas.renderMode= RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
            canvas.sortingOrder = 5 + _order++;//
        else
            canvas.sortingOrder = -2;
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;



        GameObject go = GameManager.Resource.Instantiate($"UI/SubItem/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        return Util.GetOrAddComponent<T>(go);
    }


    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _scene = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.Resource.Instantiate($"UI/Popup/{name}");
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
        GameManager.Resource.Destroy(popup.gameObject);
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