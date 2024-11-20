using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    public abstract class UI_Base : MonoBehaviour
    {
        protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

        // 설정 해상도
        float setWidth = 1440f;
        float setHeight = 2960f;

        private CanvasScaler canvasScaler;

        public virtual void Init()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            SetResolution();
        }

        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// 캔버스 안 UI 구분하고 얻어옴
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">enum의 타입</param>
        protected void Bind<T>(Type type) where T : UnityEngine.Object
        {
            string[] names = Enum.GetNames(type);

            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
            _objects.Add(typeof(T), objects);

            for (int i = 0; i < names.Length; i++)
            {
                if (typeof(T) == typeof(GameObject))
                    objects[i] = Util.FindChild(gameObject, names[i], true);
                else
                    objects[i] = Util.FindChild<T>(gameObject, names[i], true);

                if (objects[i] == null)
                    Debug.LogError($"Failed to bind : {names[i]} on {gameObject.name}");
            }
        }
        protected T Get<T>(int idx) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objects = null;
            if (_objects.TryGetValue(typeof(T), out objects) == false)
                return null;

            return objects[idx] as T;
        }

        protected GameObject GetGameObject(int idx)
        {
            return Get<GameObject>(idx);
        }

        protected TMP_Text GetText(int idx)
        {
            return Get<TMP_Text>(idx);
        }

        protected Button GetButton(int idx)
        {
            return Get<Button>(idx);
        }

        protected Image GetImage(int idx)
        {
            return Get<Image>(idx);
        }

        public static void BindEvent(GameObject go, Action<UnityEngine.EventSystems.PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
        {
            UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

            switch (type)
            {
                case Define.UIEvent.Click:
                    evt.OnClickHandler -= action;
                    evt.OnClickHandler += action;
                    break;

                case Define.UIEvent.Drag:
                    evt.OnDragHandler -= action;
                    evt.OnDragHandler += action;
                    break;
            }

            evt.OnDragHandler += ((PointerEventData data) => { evt.gameObject.transform.position = data.position; });
        }

        public void SetResolution()
        {
            float deviceWidth = Screen.width;
            float deviceHeight = Screen.height;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(setWidth, setHeight);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            if (setWidth / setHeight < deviceWidth / deviceHeight)
            {
                canvasScaler.matchWidthOrHeight = 1f;
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 0f;
            }
        }
    }
}
