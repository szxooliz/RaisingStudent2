using System;
using System.Collections;
using System.Collections.Generic;
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
        private readonly int STAT_STANDARD = 80;

        // 대학원생 엔딩 기준값
        private readonly int POSTGRAD_INTELI = 100; // 지력
        private readonly int POSTGRAD_OTAKU = 70; // 덕력
        private readonly int POSTGRAD_STRENGTH = 0; // 체력
        private readonly int POSTGRAD_CHARMING = 50; // 매력

        // 자체휴강 시 감소하는 스트레스 값(차례로 대성공, 성공, 대실패)
        private List<int> StressRestList = new() { 80, 60, 30 };
        #endregion

        public ActivityData activityData; // 활동 하나의 데이터, 결과 전달용
        public eEndingName endingName;
        static GameManager s_instance;
        public static GameManager Instance { get { Init(); return s_instance; } }
        GameManager() { }

        void Start()
        {
            Init();
            SoundManager.Instance.Play(eSound.BGM_Main);
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

        /// <summary>
        /// 활동별 실행 내용
        /// </summary>
        /// <param name="actType"></param>
        /// <returns></returns>
        public void StartActivity(eActivityType actType)
        {
            if (actType == eActivityType.MaxCount) 
            { 
                Debug.LogError("유효한 활동이 아닙니다");
                return;
            }
            activityData = SetNewActivityData(actType);
            //LogManager.Instance.GetNewLogGroup(Util.GetActivityTitle(actType));
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
            activityData.statValues = activityValues;

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
                DataManager.Instance.playerData.StatsAmounts[(int)activityData.statNames[i]] += value;
            }
        }

        /// <summary> 스트레스 증감 함수 </summary>
        /// <param name="amount">음수: 감소</param>
        public void UpdateStress()
        {
            DataManager.Instance.playerData.StressAmount += activityData.stressValue;
        }

        /// <summary> 다음 턴으로 넘김 처리 </summary>
        public void NextTurn()
        {
            DataManager.Instance.playerData.CurrentTurn++;

            // 턴 수를 3으로 나눈 나머지로 상/중/하순 결정
            DataManager.Instance.playerData.CurrentThird = (eThirds)(DataManager.Instance.playerData.CurrentTurn % 3);

            // 상순이 되면 다음 달로 넘어감
            if (DataManager.Instance.playerData.CurrentThird == 0)
            {
                if (DataManager.Instance.playerData.CurrentMonth == eMonths.Jun)
                    DataManager.Instance.playerData.CurrentMonth = eMonths.Sep;
                else DataManager.Instance.playerData.CurrentMonth++;
            }
        }

        public void CheckEndingTurn()
        {
            // 엔딩으로 넘어가기
            
            endingName = CheckEnding();
            Ending ending = new Ending(endingName, DataManager.Instance.playerData);
            DataManager.Instance.persistentData.AddOrUpdateEnding(ending);
            SceneManager.LoadScene("EndingScene");
        }

        /// <summary> 엔딩 계산 함수 </summary>
        public eEndingName CheckEnding()
        {
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

            if (highStatsCount >= 2)
            {
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
                }
            }
            
            // 위에서 하나도 안 걸리면
            return eEndingName.HomeProtector;
        }

    }
}