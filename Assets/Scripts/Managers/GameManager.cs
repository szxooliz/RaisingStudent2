using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class GameManager : MonoBehaviour
    {
        #region constant 기획 조정용
        // 코드 작성 시 증가 시에는 양수, 감소 시에는 음수로 값 설정
        private const float STRESS_CLASS = 25f;
        private const float STRESS_GAME = 20f;
        private const float STRESS_WORKOUT = 20f;
        private const float STRESS_CLUB = 20f;
        private List<int> StressRestList = new() { 80, 60, 30 };
        #endregion
        public ActivityData activityData; // 활동 하나의 데이터, 결과 전달용

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
            }
        }

        /// <summary>
        /// 활동별 실행 내용
        /// </summary>
        /// <param name="actType"></param>
        /// <returns></returns>
        public void StartActivity(int actType)
        {
            if (actType == (int)eActivityType.MaxCount) 
            { 
                Debug.LogError("유효한 활동이 아닙니다");
                return;
            }
            activityData = SetNewActivityData((eActivityType)actType);

            // 자체 휴강일 때는 업데이트할 스탯 없음
            if (actType != (int)eActivityType.Rest) UpdateStats();

            UpdateStress();

            // 활동마다 데이터 Save - TODO : 출시 때 주석 해제
            // DataManager.Instance.SaveAllData();
        }

        /// <summary>
        /// 활동에 대한 스트레스, 스탯 정보 설정 - 하드코딩
        /// </summary>
        /// <param name="activityType">메인 화면에서 선택한 활동 타입</param>
        /// <returns></returns>
        public ActivityData SetNewActivityData(eActivityType activityType)
        {
            ActivityData activityData = new ActivityData();
            activityData.statValues = new List<int>() { 10, 5 }; // 임시값

            switch (activityType)
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
        /// 스트레스에 따른 활동 결과 리턴
        /// </summary>
        /// <returns></returns>
        public eResultType GetResult()
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

        /// <summary>
        /// 활동 결과에 따른 스탯 획득량 배수 리턴
        /// </summary>
        /// <param name="resultType"></param>
        /// <returns></returns>
        public float GetStatMultiplier(eResultType resultType)
        {
            switch(resultType)
            {
                case eResultType.Failure:
                    return 0.5f;
                case eResultType.Success:
                    return 1f;
                default:
                    return 1.5f;
            }
        }

        /// <summary>
        /// 결과에 따른 스탯 획득량 반영
        /// </summary>
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

        /// <summary>
        /// 스트레스 증감 함수
        /// </summary>
        /// <param name="amount">음수: 감소</param>
        public void UpdateStress()
        {
            DataManager.Instance.playerData.StressAmount += activityData.stressValue;
        }

        /// <summary>
        /// 다음 턴으로 넘김 처리
        /// </summary>
        public void NextTurn()
        {
            // TODO : 엔딩으로 넘어가기
            if (DataManager.Instance.playerData.CurrentTurn == 24) return; // 미구현

            DataManager.Instance.playerData.CurrentTurn++;

            // 턴 수를 3으로 나눈 나머지로 상/중/하순 결정
            DataManager.Instance.playerData.CurrentThird = (eThirds)(DataManager.Instance.playerData.CurrentTurn % 3);

            // 상순이 되면 다음 달로 넘어감
            if (DataManager.Instance.playerData.CurrentThird == 0)
            {
                if (DataManager.Instance.playerData.CurrentMonth == eMonths.Jun) DataManager.Instance.playerData.CurrentMonth = eMonths.Sep;
                else DataManager.Instance.playerData.CurrentMonth++;
            }
        }

        /*
        IEnumerator LoadDatas()
        {
            Coroutine cor1 = StartCoroutine(s_instance._data.RequestAndSetDayDatas(DayDatasURL));
            Coroutine cor2 = StartCoroutine(s_instance._data.RequestAndSetRandEventDatas(RandEventURL));
            Coroutine cor3 = StartCoroutine(s_instance._data.RequestAndSetItemDatas(MerchantURL));

            yield return cor1;
            yield return cor2;
            yield return cor3;

        }*/

    }
}