using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        /// <summary>
        /// 로드한 적 있는 오브젝트의 캐시, 경로를 key로 이용
        /// </summary>
        Dictionary<string, Object> _cache = new Dictionary<string, Object>();

        ResourceManager() { }

        private string GetPrefabPath(string path) => $"Prefabs/{path}";

        public T Load<T>(string path) where T : Object
        {
            string name = path;

            Object obj;

            if (_cache == null)
            {
                _cache = new Dictionary<string, Object>();
            }

            //캐시에 존재 -> 캐시에서 반환
            if (_cache.TryGetValue(name, out obj))
                return obj as T;

            //캐시에 없음 -> 로드하여 캐시에 저장 후 반환
            obj = Resources.Load<T>(path);
            _cache.Add(name, obj);

            return obj as T;
        }

        public GameObject Instantiate(string path, Transform parent = null)
        {
            GameObject original = Load<GameObject>($"Prefabs/{path}");
            if (original == null)
            {
                Debug.Log($"Failed to load prefab : {path}");
            }


            GameObject go = Object.Instantiate(original, parent);
            go.name = original.name;

            return go;
        }

        public void Destroy(GameObject go)
        {
            if (go == null)
                return;


            Object.Destroy(go);
        }
    }
}
