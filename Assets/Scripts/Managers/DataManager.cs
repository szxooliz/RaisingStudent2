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
        #region constant 기획 조정용
        // 증가 시에는 양수, 감소 시에는 음수로 값 설정
        private const float STRESS_CLASS = 25f;
        private const float STRESS_GAME = 20f;
        private const float STRESS_WORKOUT = 20f;
        private const float STRESS_CLUB = 20f;
        private List<int> StressRestList = new() { 80, 60, 30 };
        #endregion

        // 로드한 적 있는 DataTable (Table 명을  Key1 데이터 ID를 Key2로 사용)
        Dictionary<string, Dictionary<long, SheetData>> _cache = new Dictionary<string, Dictionary<long, SheetData>>();
        // 로드한 적 있는 Sprite
        Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        public PlayerData playerData; 
        public PersistentData persistentData;

        public ActivityData activityData; // 활동 하나의 데이터, 결과 전달용

        #region Singleton
        private DataManager()
        { }
        #endregion

        public override void Init()
        {
            LoadAllData();
            LoadSheetDatas();

            // 미리 EventResult 전부 딕셔너리에 저장
            for (int i = 0; ; i++)
            {
                try
                {
                    EventResult eventResult = GetData<EventResult>(i);
                    EventManager.Instance.EventResults.Add(eventResult.ScriptIndex, eventResult);
                }
                catch
                {
                    break;
                }
            }
            
            // MARK-: EndingList 객체 생성 함수
            persistentData.Initialize();
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
        #endregion

        #region JSON Data Load & Save
        public T LoadData<T>(string path) where T : new()
        {
#if UNITY_EDITOR
            DeleteSaveFile(path);
#endif
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
        public void LoadAllData()
        {
            string playerData_path = Path.Combine(Application.persistentDataPath, "PlayerData.json");
            string persistentData_path = Path.Combine(Application.persistentDataPath, "PersistentData.json");
            playerData = LoadData<PlayerData>(playerData_path);
            persistentData = LoadData<PersistentData>(persistentData_path);
        }
        public void SaveAllData()
        {
            string playerData_path = Path.Combine(Application.persistentDataPath, "PlayerData.json");
            string persistentData_path = Path.Combine(Application.persistentDataPath, "PersistentData.json");
            SaveData<PlayerData>(playerData_path, playerData);
            SaveData<PersistentData>(persistentData_path, persistentData);
        }

        #endregion

        /// <summary>
        /// 활동에 대한 스트레스, 스탯 정보 설정 - 하드코딩
        /// </summary>
        /// <param name="activityType">메인 화면에서 선택한 활동 타입</param>
        /// <returns></returns>
        public ActivityData SetNewActivityData(eActivityType activityType)
        {
            activityData = new ActivityData();
            activityData.statValues = new List<int>() { 10, 5 }; // 임시값

            switch(activityType)
            {
                case eActivityType.Rest:
                    activityData.activityType = activityType;
                    // 자체휴강만 랜덤으로 대성공/성공/대실패 여부 결정
                    int prob = UnityEngine.Random.Range(0, 3);
                    activityData.resultType = (eResultType)prob;
                    activityData.stressValue = -StressRestList[prob]; ;
                    break;
                case eActivityType.Class:
                    activityData.activityType = activityType;
                    activityData.statNames.Add(eStatName.Inteli);
                    activityData.statNames.Add(eStatName.Strength);
                    activityData.stressValue = STRESS_CLASS;
                    break;
                case eActivityType.Game:
                    activityData.activityType = activityType;
                    activityData.statNames.Add(eStatName.Otaku);
                    activityData.statNames.Add(eStatName.Inteli);
                    activityData.stressValue = STRESS_GAME;
                    break;
                case eActivityType.Workout:
                    activityData.activityType = activityType;
                    activityData.statNames.Add(eStatName.Strength);
                    activityData.statNames.Add(eStatName.Charming);
                    activityData.stressValue = STRESS_WORKOUT;
                    break;
                case eActivityType.Club:
                    activityData.activityType = activityType;
                    activityData.statNames.Add(eStatName.Charming);
                    activityData.statNames.Add(eStatName.Otaku);
                    activityData.stressValue = STRESS_CLUB;
                    break;
            }
        
            return activityData;
        }

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