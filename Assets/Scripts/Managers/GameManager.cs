using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Client.SystemEnum;

namespace Client
{
    public class GameManager : MonoBehaviour
    {
        #region 기획 조정용 상수

        // 활동으로 얻는 스트레스 양
        private readonly float STRESS_CLASS = 25f;
        private readonly float STRESS_GAME = 20f;
        private readonly float STRESS_WORKOUT = 20f;
        private readonly float STRESS_CLUB = 20f;

        // 활동으로 얻는 주스탯, 부스탯 증가량
        private List<int> activityValues = new List<int>() { 10, 5 };

        // 자체휴강 결과 확률 - 대실패는 얘네 바꾸면 자동으로 적용됩니다
        private readonly int REST_BIGSUCCESS = 40;
        private readonly int REST_SUCCESS = 50;

        // 엔딩 결과 산출 기준값
        private readonly int STAT_STANDARD = 90;

        // 대학원생 엔딩 기준값
        private readonly int POSTGRAD_INTELI = 100; // 지력
        private readonly int POSTGRAD_OTAKU = 80; // 덕력
        private readonly int POSTGRAD_STRENGTH = 80; // 체력
        private readonly int POSTGRAD_CHARMING = 0; // 매력

        // 자체휴강 시 감소하는 스트레스 값(차례로 대성공, 성공, 대실패)
        private List<int> StressRestList = new() { 80, 60, 30 };

        static readonly Dictionary<(int, int), eEndingName> endingMap = new()
        {
            { ((int)eStatName.Inteli, (int)eStatName.Charming), eEndingName.CorporateSI },
            { ((int)eStatName.Inteli, (int)eStatName.Otaku),    eEndingName.GameCompany },
            { ((int)eStatName.Otaku,  (int)eStatName.Charming), eEndingName.VirtualYoutuber },
            { ((int)eStatName.Otaku,  (int)eStatName.Strength), eEndingName.ProGamer },
        };

        #endregion

        public long[] tempStat { get; } = new long[4];
        public float tempStress = 0;

        public ActivityData activityData; // 활동 하나의 데이터, 결과 전달용
        public eEndingName endingName;
        static GameManager s_instance;
        public static GameManager Instance { get { Init(); return s_instance; } }
        GameManager() { }

        void Start()
        {
            Init();
            DataManager.Instance.PreDataMapping();
        }

        static void Init()
        {
            if (s_instance == null)
            {
                GameObject go = GameObject.Find("@GameManager");
                if (go == null)
                {
                    go = new GameObject { name = "@GameManager" };
                    go.AddComponent<GameManager>();
                    Debug.Log("GameManager 생성");
                }

                DontDestroyOnLoad(go);
                s_instance = go.GetComponent<GameManager>();

                DataManager.Instance.Init();
                SoundManager.Instance.Init();
                EventManager.Instance.Init();
            }
        }

        /// <summary> 활동별 실행 내용  </summary>
        public void StartActivity(eActivityType actType)
        {
            if (actType == eActivityType.MaxCount) 
            { 
                Debug.LogError("유효한 활동이 아닙니다");
                return;
            }
            activityData = SetNewActivityData(actType);
            LogManager.Instance.GetNewClusterGroup(Util.GetActivityTitle(actType));

            // 자체 휴강일 때는 업데이트할 스탯 없음
            if (actType != (int)eActivityType.Rest) UpdateStats();

            UpdateStress();

            DataManager.Instance.SaveAllData();
        }

        /// <summary> 활동에 대한 스트레스, 스탯 정보 설정 - 하드코딩 </summary>
        /// <param name="activityType">메인 화면에서 선택한 활동 타입</param>
        public ActivityData SetNewActivityData(eActivityType activityType)
        {
            ActivityData activityData = new ActivityData(activityType);
            activityData.statValues = activityValues.ToList();
            Debug.Log($"<color=lemonchiffon>활동 데이터 스탯값 확인 {activityValues[0]} {activityValues[1]}</color>");

            switch (activityType)
            {
                case eActivityType.Rest:
                    eResultType eResult = GetRestResult();
                    activityData.resultType = eResult;
                    activityData.stressValue = -StressRestList[(int)eResult]; ;
                    break;
                case eActivityType.Class:
                    activityData.statNames.Add(eStatName.Inteli);
                    activityData.statNames.Add(eStatName.Strength);
                    activityData.stressValue = STRESS_CLASS;
                    break;
                case eActivityType.Game:
                    activityData.statNames.Add(eStatName.Otaku);
                    activityData.statNames.Add(eStatName.Inteli);
                    activityData.stressValue = STRESS_GAME;
                    break;
                case eActivityType.Workout:
                    activityData.statNames.Add(eStatName.Strength);
                    activityData.statNames.Add(eStatName.Charming);
                    activityData.stressValue = STRESS_WORKOUT;
                    break;
                case eActivityType.Club:
                    activityData.statNames.Add(eStatName.Charming);
                    activityData.statNames.Add(eStatName.Otaku);
                    activityData.stressValue = STRESS_CLUB;
                    break;
            }
            if (activityType != eActivityType.Rest)
                Debug.Log($"<color=green> 스탯 증가값 {activityData.statNames[0]}: {activityData.statValues[0]} / {activityData.statNames[1]}: {activityData.statValues[1]}</color>");

            return activityData;
        }

        eResultType GetRestResult()
        {
            int prob = UnityEngine.Random.Range(0, 100);

            if (prob <= REST_BIGSUCCESS) { return eResultType.BigSuccess; }
            else if (prob <= REST_BIGSUCCESS + REST_SUCCESS) { return eResultType.Success; }
            else { return eResultType.Failure; }
        }

        /// <summary> 스트레스에 따른 활동 결과 리턴 </summary>
        eResultType GetResult()
        {
            // 이번 활동 결과의 확률을 정함
            int prob = UnityEngine.Random.Range(0, 100);

            if (DataManager.Instance.playerData.StressAmount >= 70)
            {
                if (prob <= 50) { return eResultType.Failure; }
                else if (prob <= 95) { return eResultType.Success; }
                else { return eResultType.BigSuccess; }
            }
            else if (DataManager.Instance.playerData.StressAmount >= 40)
            {
                if (prob <= 80) { return eResultType.Success; }
                else { return eResultType.BigSuccess; }
            }
            else
            {
                if (prob <= 60) { return eResultType.Success; }
                else { return eResultType.BigSuccess; }
            }
        }

        /// <summary> 활동 결과에 따른 스탯 획득량 배수 리턴 </summary>
        public float GetStatMultiplier(eResultType resultType)
        {
            if (resultType == eResultType.Failure) return 0.5f;
            else if (resultType == eResultType.Success) return 1f;
            else return 1.5f;
        }

        /// <summary> 결과에 따른 스탯 획득량 반영 </summary>
        public void UpdateStats()
        {
            activityData.resultType = GetResult();
            float multiplier = GetStatMultiplier(activityData.resultType);

            for (int i = 0; i < activityData.statNames.Count; i++)
            {
                // 소수점 올림 처리
                int value = (int)Math.Ceiling(activityData.statValues[i] * multiplier);
                activityData.statValues[i] = value;

                // 스탯 증감 처리
                //DataManager.Instance.playerData.StatsAmounts[(int)activityData.statNames[i]] += value;
            }
        }

        /// <summary> 스트레스 값 임시 변수에 적용 </summary>
        /// <param name="amount">음수: 감소</param>
        public void UpdateStress()
        {
            //DataManager.Instance.playerData.StressAmount += activityData.stressValue;
            tempStress += activityData.stressValue;

        }

        /// <summary> 다음 턴으로 넘김 처리 </summary>
        public void NextTurn()
        {
            DataManager.Instance.playerData.CurrentTurn++;
        }

        public void NextMonthandTerm()
        {
            Debug.Log($"<color=green>턴 계산 전: {DataManager.Instance.playerData.CurrentMonth} {DataManager.Instance.playerData.CurrentThird} ({DataManager.Instance.playerData.CurrentTurn})</color>");

            // 이전 턴에 하순이었을 때 다음 달로 넘어감
            if (DataManager.Instance.playerData.CurrentThird == eThirds.Third)
            {
                if (DataManager.Instance.playerData.CurrentMonth == eMonths.Jun)
                    DataManager.Instance.playerData.CurrentMonth = eMonths.Sep;
                else DataManager.Instance.playerData.CurrentMonth++;
            }

            // 턴 수를 3으로 나눈 나머지로 상/중/하순 결정
            DataManager.Instance.playerData.CurrentThird = (eThirds)(DataManager.Instance.playerData.CurrentTurn % 3);

            Debug.Log($"<color=green>턴 계산 후: {DataManager.Instance.playerData.CurrentMonth} {DataManager.Instance.playerData.CurrentThird} ({DataManager.Instance.playerData.CurrentTurn})</color>");
        }

        public void CheckEndingTurn()
        {
            // 엔딩으로 넘어가기
            DataManager.Instance.ApplyTurnStat();

            endingName = CheckEnding();
            Ending ending = new Ending(endingName, DataManager.Instance.playerData);
            DataManager.Instance.persistentData.AddOrUpdateEnding(ending);
            SceneManager.LoadScene("EndingScene");
        }

        /// <summary> 엔딩 계산 함수 </summary>
        public eEndingName CheckEnding()
        {
            Debug.Log($"엔딩 계산 타이밍");
            int highStatsCount = 0;
            bool[] highStats = { false, false, false, false };

            for (int i = 0; i < 4; i++)
            {
                if (DataManager.Instance.playerData.StatsAmounts[i] >= STAT_STANDARD)
                {
                    highStats[i] = true;
                    highStatsCount++;
                }
            }

            bool isPostgrad = DataManager.Instance.playerData.StatsAmounts[(int)eStatName.Inteli] >= POSTGRAD_INTELI
                               && DataManager.Instance.playerData.StatsAmounts[(int)eStatName.Otaku] >= POSTGRAD_OTAKU
                               && DataManager.Instance.playerData.StatsAmounts[(int)eStatName.Strength] >= POSTGRAD_STRENGTH
                               && DataManager.Instance.playerData.StatsAmounts[(int)eStatName.Charming] >= POSTGRAD_CHARMING;

            if (isPostgrad)
            {
                // 대학원생
                return eEndingName.GraduateStudent;
            }

            // 스탯 정렬 (값 ↓, 우선순위 ↑)
            var ordered = DataManager.Instance.playerData.StatsAmounts
                .Select((value, index) => new { value, index })
                .Where(x => x.value >= STAT_STANDARD) // 기준치 이상만
                .OrderByDescending(x => x.value)
                .ThenBy(x => x.index)
                .ToList();

            if (ordered.Count >= 2)
            {
                int first = ordered[0].index;
                int second = ordered[1].index;

                var key = (Math.Min(first, second), Math.Max(first, second));
                if (endingMap.TryGetValue(key, out var ending))
                    return ending;
                /*
                if (highStats[(int)eStatName.Inteli] && highStats[(int)eStatName.Charming])
                {
                    // 대기업
                    return eEndingName.CorporateSI;
                }
                else if (highStats[(int)eStatName.Inteli] && highStats[(int)eStatName.Otaku])
                {
                    // 게임회사
                    return eEndingName.GameCompany;
                }
                else if (highStats[(int)eStatName.Otaku] && highStats[(int)eStatName.Charming])
                {
                    // 버부버
                    return eEndingName.VirtualYoutuber;
                }
                else if (highStats[(int)eStatName.Otaku] && highStats[(int)eStatName.Strength])
                {
                    // 프로게
                    return eEndingName.ProGamer;
                }*/
            }
            
            // 위에서 하나도 안 걸리면
            return eEndingName.HomeProtector;
        }

        /// <summary> 여름 기간에 해당되면 true 반환 </summary>
        public bool IsSummerTerm()
        {
            return DataManager.Instance.playerData.CurrentMonth > eMonths.Apr && DataManager.Instance.playerData.CurrentMonth < eMonths.Nov;
        }

    }
}