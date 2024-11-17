using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Client
{
    public class DataManager : Singleton<DataManager>
    {
        /// 로드한 적 있는 DataTable (Table 명을  Key1 데이터 ID를 Key2로 사용)
        Dictionary<string, Dictionary<long, SheetData>> _cache = new Dictionary<string, Dictionary<long, SheetData>>();

        #region Singleton
        private DataManager()
        { }
        #endregion

        public override void Init()
        {
            DataLoad();
        }

        public void DataLoad()
        {
            // 현재 어셈블리 내에서 SheetData를 상속받는 모든 타입을 찾음
            var sheetDataTypes = Assembly.GetExecutingAssembly().GetTypes()
                                         .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(SheetData)));

            List<SheetData> instances = new List<SheetData>();

            foreach (var type in sheetDataTypes)
            {

                // 각 타입에 대해 인스턴스 생성
                SheetData instance = (SheetData)Activator.CreateInstance(type);
                if (instance == null)
                {
                    continue;
                }
                Debug.Log(type.Name);
                Dictionary<long, SheetData> sheet = instance.LoadData();

                if (_cache.ContainsKey(type.Name) == false)
                {
                    _cache.Add(type.Name, sheet);
                }

            }
        }

        public T GetData<T>(long Index) where T : SheetData
        {
            string key = typeof(T).ToString();
            key = key.Replace("Client.", "");

            if (!_cache.ContainsKey(key))
            {
                Debug.LogError($"{key} 데이터 테이블은 존재하지 않습니다.");
                return null;
            }
            if (!_cache[key].ContainsKey(Index))
            {
                Debug.LogError($"{key} 데이터에 ID {Index}는 존재하지 않습니다.");
                return null;
            }
            T returnData = _cache[key][Index] as T;
            if (returnData == null)
            {
                Debug.LogError($"{key} 데이터에 ID {Index}는 존재하지만 {key}타입으로 변환 실패했습니다.");
                return null;

            }

            return returnData;
        }

        public void SetData<T>(int id, T data) where T : SheetData
        {
            string key = typeof(T).ToString();
            key = key.Replace("Client.", "");

            if (_cache.ContainsKey(key))
            {
                Debug.LogWarning($"{key} 데이터 테이블은 이미 존재합니다.");
            }
            else
            {
                _cache.Add(key, new Dictionary<long, SheetData>());
            }

            if (_cache[key].ContainsKey(id))
            {
                Debug.LogWarning($"{key} 타입 ID: {id} 칼럼은 이미 존재합니다. !(주의) 게임 중 데이터 칼럼을 변경할 수 없습니다!");
            }
            else
            {
                _cache[key].Add(id, data);
            }
        }
    }
}