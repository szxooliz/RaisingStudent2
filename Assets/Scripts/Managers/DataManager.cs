using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static Client.Define;

namespace Client
{
    public class DataManager : Singleton<DataManager>
    {
        // 로드한 적 있는 DataTable (Table 명을  Key1 데이터 ID를 Key2로 사용)
        Dictionary<string, Dictionary<long, SheetData>> _cache = new Dictionary<string, Dictionary<long, SheetData>>();
        // 로드한 적 있는 Sprite
        Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        public PlayerData playerData; // 플레이어 데이터
        public PersistentData persistentData = new PersistentData(); // 엔딩 목록

        public ActivityData activityData = new ActivityData(); // 활동 하나의 데이터, 결과 전달용

        #region Singleton
        private DataManager()
        { }
        #endregion

        public override void Init()
        {
            DataLoad();
            LoadPlayerData();
        }

        #region SheetData Load & Save
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
        public void LoadPlayerData()
        {
            string path = Path.Combine(Application.persistentDataPath, "PlayerData.json");

#if UNITY_EDITOR
            // 에디터 내부 테스트용 
            DeleteSaveFile();
#endif
            if (!File.Exists(path))
            {
                Debug.LogError("Player Data 없음, 새로 생성");
                playerData = new PlayerData();
                SavePlayerData();
            }

            FileStream fileStream = new FileStream(path, FileMode.Open);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();
            string jsonData = Encoding.UTF8.GetString(data);
            playerData = JsonUtility.FromJson<PlayerData>(jsonData);

            Debug.Log("Player Data Load 성공");
        }

        public void SavePlayerData() 
        {
            string path = Path.Combine(Application.persistentDataPath, "PlayerData.json");

            string jsonData = JsonUtility.ToJson(playerData, true);
            FileStream fileStream = new FileStream(path, FileMode.Create);
            byte[] data = Encoding.UTF8.GetBytes(jsonData);
            fileStream.Write(data, 0, data.Length);
            fileStream.Close();

            Debug.Log("Player Data Save 성공");

        }

        void DeleteSaveFile()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json"); // 저장된 파일 이름
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Save file deleted.");
            }
            else
            {
                Debug.Log("Save file does not exist.");
            }
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
                // 증가 시에는 양수, 감소 시에는 음수로 값 설정
                case eActivityType.Rest:
                    activityData.activityType = eActivityType.Rest;
                    activityData.stressValue = -10f; // 임시값
                    break;
                case eActivityType.Class:
                    activityData.activityType = eActivityType.Class;
                    activityData.statNames.Add(eStatName.Inteli);
                    activityData.statNames.Add(eStatName.Strength);
                    activityData.stressValue = 30f; // 임시값
                    break;
                case eActivityType.Game:
                    activityData.activityType = eActivityType.Class;
                    activityData.statNames.Add(eStatName.Otaku);
                    activityData.statNames.Add(eStatName.Inteli);
                    activityData.stressValue = 10f; // 임시값
                    break;
                case eActivityType.Workout:
                    activityData.activityType = eActivityType.Class;
                    activityData.statNames.Add(eStatName.Strength);
                    activityData.statNames.Add(eStatName.Charming);
                    activityData.stressValue = 20f; // 임시값
                    break;
                case eActivityType.Club:
                    activityData.activityType = eActivityType.Class;
                    activityData.statNames.Add(eStatName.Charming);
                    activityData.statNames.Add(eStatName.Otaku);
                    activityData.stressValue = 10f; // 임시값
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