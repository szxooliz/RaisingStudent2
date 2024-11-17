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
        protected Dictionary<Type, UnityEngine.Object[]> _objeects = new Dictionary<Type, UnityEngine.Object[]>();

        public abstract void Init();

        private void Awake()
        {
            Init();
        }


        protected void Bind<T>(Type type) where T : UnityEngine.Object
        {
            string[] names = Enum.GetNames(type);

            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
            _objeects.Add(typeof(T), objects);

            for (int i = 0; i < names.Length; i++)
            {
                if (typeof(T) == typeof(GameObject))
                    objects[i] = Util.FindChild(gameObject, names[i], true);
                else
                    objects[i] = Util.FindChild<T>(gameObject, names[i], true);
            }
        }
        protected T Get<T>(int idx) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objects = null;
            if (_objeects.TryGetValue(typeof(T), out objects) == false)
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
    }
}
