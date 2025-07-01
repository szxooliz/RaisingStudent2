using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class DataManager : Singleton<DataManager>
    {

        // 로드한 적 있는 DataTable (Table 명을  Key1 데이터 ID를 Key2로 사용)
        Dictionary<string, Dictionary<long, SheetData>> _cache = new Dictionary<string, Dictionary<long, SheetData>>();
        // 로드한 적 있는 Sprite
        Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        // key1 : EventNum , key2: index
        Dictionary<long, Dictionary<long, EventScript>> _eventScriptDict = new();
        Dictionary<long, EventResult>                   _eventResultDict = new();
        Dictionary<string, CharacterFace>               _charFaceDict    = new();
        Dictionary<long, Dictionary<long, EndingScript>> _endingScriptDict = new();

        public Dictionary<long, Dictionary<long, EventScript>> EventScriptDict => _eventScriptDict;
        public Dictionary<long, EventResult>                   EventResultDict => _eventResultDict; // key : ScriptIndex
        public Dictionary<string, CharacterFace>               CharFaceDict    => _charFaceDict;
        public Dictionary<long, Dictionary<long, EndingScript>>EndingScriptDict => _endingScriptDict;

        public PlayerData playerData; 
        public PersistentData persistentData;

        #region Singleton
        private DataManager()
        { }
        #endregion

        public override void Init()
        {
            LoadPersistentData();
            DeleteAllData();
            LoadSheetDatas();

            // 미리 EventResult, CharacterFace, EndingScriptDict 전부 딕셔너리에 저장
            var eventResultList = GetDataList<EventResult>();
            foreach (var _eventResult in eventResultList)
            {
                var eventResult = _eventResult as EventResult;
                _eventResultDict.TryAdd(eventResult.ScriptIndex, eventResult);
            }
            var charFaceList = GetDataList<CharacterFace>();
            foreach (var _charFace in charFaceList)
            {
                var charFace = _charFace as CharacterFace;
                _charFaceDict.TryAdd(charFace.Character, charFace);
            }
        }

        #region SheetData Load & Save
        public void LoadSheetDatas()
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
                Debug.Log("SheetData Load : " + type.Name);
                Dictionary<long, SheetData> sheet = instance.LoadData();

                if (_cache.ContainsKey(type.Name) == false)
                {
                    _cache.Add(type.Name, sheet);
                }
                SetTypeData(type.Name);
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

        public List<SheetData> GetDataList<T>() where T : SheetData
        {
            string typeName = typeof(T).Name;
            if (_cache.ContainsKey(typeName) == false)
            {
                Debug.LogWarning($"DataManager : {typeName} 타입 데이터가 존재하지 않습니다.");

                return null;
            }
            return _cache[typeName].Values.ToList();
        }
        private void SetTypeData(string data)
        {
            if (typeof(EventScript).ToString().Contains(data)) { SetEventScriptsMap(); return; }
            if (typeof(EndingScript).ToString().Contains(data)) { SetEndingScriptsMap(); return; }
        }
        /// <summary>
        /// 이벤트 아이디별 스크립트 데이터
        /// </summary>
        private void SetEventScriptsMap()
        {
            string key = typeof(EventScript).Name;
            if (_cache.ContainsKey(key) == false)
                return;

            var eventScriptDict = _cache[key];
            if (eventScriptDict is null) return;

            foreach (var kvp in eventScriptDict)
            {
                var eventScript = kvp.Value as EventScript;

                if (!_eventScriptDict.ContainsKey(eventScript.EventNum))
                    _eventScriptDict.Add(eventScript.EventNum, new Dictionary<long, EventScript>());

                if (!_eventScriptDict[eventScript.EventNum].ContainsKey(eventScript.index))
                    _eventScriptDict[eventScript.EventNum].Add(eventScript.index, eventScript);
            }
        }
        private void SetEndingScriptsMap()
        {
            string key = typeof(EndingScript).Name;
            if (_cache.ContainsKey(key) == false)
                return;

            var endingScriptDict = _cache[key];
            if (endingScriptDict is null) return;

            foreach (var kvp in endingScriptDict)
            {
                var endingScript = kvp.Value as EndingScript;

                if (!_endingScriptDict.ContainsKey(endingScript.EndingNum))
                    _endingScriptDict.Add(endingScript.EndingNum, new Dictionary<long, EndingScript>());

                if (!_endingScriptDict[endingScript.EndingNum].ContainsKey(endingScript.index))
                    _endingScriptDict[endingScript.EndingNum].Add(endingScript.index, endingScript);
            }
            Debug.Log($"엔딩스크립트 모음.. {_endingScriptDict.Count}");
            for (int i = 0; i < 6; i++)
            {
                Debug.Log($"<color=red>엔딩 {i} 딕셔너리 안에.. {_endingScriptDict[i].Count}</color>");
                Debug.Log($"<color=red>엔딩 {i} 딕셔너리 안에.. min key {_endingScriptDict[i].Keys.Min()}</color>");

            }
        }

        #endregion

        #region JSON Data Load & Save

        public T LoadData<T>(string path) where T : new()
        {
            if (!File.Exists(path))
            {
                Debug.LogError("데이터 없음, 새로 생성: " + path);
                T data = new T();
                SaveData(path, data);
                return data;
            }

            FileStream fileStream = new FileStream(path, FileMode.Open);
            byte[] fileData = new byte[fileStream.Length];
            fileStream.Read(fileData, 0, fileData.Length);
            fileStream.Close();
            string jsonData = Encoding.UTF8.GetString(fileData);

            Debug.Log("데이터 Load 성공: " + path);
            return JsonUtility.FromJson<T>(jsonData);
        }

        public void SaveData<T>(string path, T data)
        {
            string jsonData = JsonUtility.ToJson(data, true);
            FileStream fileStream = new FileStream(path, FileMode.Create);
            byte[] fileData = Encoding.UTF8.GetBytes(jsonData);
            fileStream.Write(fileData, 0, fileData.Length);
            fileStream.Close();

            Debug.Log("데이터 Save 성공: " + path);
        }

        void DeleteSaveFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("Save file deleted.");
            }
            else
            {
                Debug.Log("Save file does not exist.");
            }
        }

        public void LoadPlayerData()
        {
            string playerData_path = $"{Application.persistentDataPath}/PlayerData.json";
            playerData = LoadData<PlayerData>(playerData_path);
        }
        public void LoadPersistentData()
        {
            string persistentData_path = $"{Application.persistentDataPath}/PersistentData.json";
            persistentData = LoadData<PersistentData>(persistentData_path);
        }
        public void LoadAllData()
        {
            LoadPlayerData();
            LoadPersistentData();
        }
        public void SaveAllData()
        {
            string playerData_path = $"{Application.persistentDataPath}/PlayerData.json";
            string persistentData_path = $"{Application.persistentDataPath}/PersistentData.json";
            SaveData<PlayerData>(playerData_path, playerData);
            SaveData<PersistentData>(persistentData_path, persistentData);
        }
        public void DeleteAllData()
        {
            string playerData_path = $"{Application.persistentDataPath}/PlayerData.json";
            string persistentData_path = $"{Application.persistentDataPath}/PersistentData.json";
#if UNITY_EDITOR
            DeleteSaveFile(playerData_path);
            DeleteSaveFile(persistentData_path);
#endif
        }

        #endregion

        /// <summary>
        /// 캐싱된 스프라이트 로드 / 반환
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public Sprite GetOrLoadSprite(string _path)
        {
            if (spriteCache.TryGetValue(_path, out Sprite cachedSprite))
            {
                // 캐싱된 스프라이트 반환
                return cachedSprite;
            }

            Sprite loadedSprite = Resources.Load<Sprite>(_path);
            if (loadedSprite == null)
            {
                throw new System.Exception($"Sprite not found at path: {_path}");
            }

            // 로드된 스프라이트를 캐싱
            spriteCache[_path] = loadedSprite;
            return loadedSprite;
        }
    }
}