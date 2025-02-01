using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Client.Define;

namespace Client
{
    public class GameManager : MonoBehaviour
    {
        static GameManager s_instance;
        public static GameManager Instance { get { Init(); return s_instance; } }
        GameManager() { }

        void Awake()
        {
            Init();
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
            if (actType == (int)ActivityType.MaxCount) 
            { 
                Debug.LogError("유효한 활동이 아닙니다");
                return;
            }
            
            DataManager.Instance.SetNewActivityData((ActivityType)actType);


            // 자체 휴강일때는 업데이트할 스탯 없음
            if (actType != (int)ActivityType.Rest) 
            { 
                UpdateStats(); 
            }

            UpdateStress();
            NextTurn();

            // 활동마다 데이터 저장 - 출시 때 주석 해제
            //Data.SavePlayerData();
        }

        /// <summary>
        /// 스트레스에 따른 활동 결과 리턴
        /// </summary>
        /// <returns></returns>
        public ResultType GetResult()
        {
            // 이번 활동 결과의 확률을 정함
            int prob = UnityEngine.Random.Range(0, 100);

            if (DataManager.Instance.playerData.stressAmount >= 70)
            {
                if (prob <= 50) { return ResultType.Failure; }
                else if (prob <= 95) { return ResultType.Success; }
                else { return ResultType.BigSuccess; }
            }
            else if (DataManager.Instance.playerData.stressAmount >= 40)
            {
                if (prob <= 80) { return ResultType.Success; }
                else { return ResultType.BigSuccess; }
            }
            else
            {
                if (prob <= 60) { return ResultType.Success; }
                else { return ResultType.BigSuccess; }
            }
        }

        /// <summary>
        /// 활동 결과에 따른 스탯 획득량 배수 리턴
        /// </summary>
        /// <param name="resultType"></param>
        /// <returns></returns>
        public float GetStatMultiplier(ResultType resultType)
        {
            switch(resultType)
            {
                case ResultType.Failure:
                    return 0.5f;
                case ResultType.Success:
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
            DataManager.Instance.activityData.resultType = GetResult();
            float multiplier = GetStatMultiplier(DataManager.Instance.activityData.resultType);

            // 소수점 올림 처리
            DataManager.Instance.activityData.stat1Value = (int)Math.Ceiling(DataManager.Instance.activityData.stat1Value * multiplier);
            DataManager.Instance.activityData.stat2Value = (int)Math.Ceiling(DataManager.Instance.activityData.stat2Value * multiplier);

            // 스탯 증감 처리
            DataManager.Instance.playerData.statsAmounts[(int)DataManager.Instance.activityData.statName1] += DataManager.Instance.activityData.stat1Value;
            DataManager.Instance.playerData.statsAmounts[(int)DataManager.Instance.activityData.statName2] += DataManager.Instance.activityData.stat2Value;
        }

        /// <summary>
        /// 스트레스 증감 함수
        /// </summary>
        /// <param name="amount">음수: 감소</param>
        public void UpdateStress()
        {
            DataManager.Instance.playerData.stressAmount += DataManager.Instance.activityData.stressValue;
        }

        /// <summary>
        /// 다음 턴으로 넘김 처리
        /// </summary>
        public void NextTurn()
        {
            // TODO : 엔딩으로 넘어가기
            if (DataManager.Instance.playerData.currentTurn == 23) return; // 미구현

            DataManager.Instance.playerData.currentTurn++;

            // 턴 수를 3으로 나눈 나머지로 상/중/하순 결정
            DataManager.Instance.playerData.currentThird = (Thirds)(DataManager.Instance.playerData.currentTurn % 3);

            // 상순이 되면 다음 달로 넘어감
            if (DataManager.Instance.playerData.currentThird == 0)
            {
                // TODO : 6월과 9월 사이 여름방학 달 추가해야 하므로 수정 필요
                if (DataManager.Instance.playerData.currentMonth == Months.Jun) DataManager.Instance.playerData.currentMonth = Months.Sep;
                else DataManager.Instance.playerData.currentMonth++;
            }
        }

        // 5. 목표 스케줄까지 남은 턴 계산

        // 6. 마지막 턴 이후에 스탯 계산해서 엔딩 결과 내기

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


        #region UI
        //public void StartSchedule()
        //{
        //    StartCoroutine(ScheduleExecuter.Inst.StartSchedule());
        //}

        //public void ShowMainStory()
        //{
        //    StartCoroutine(ShowMainStoryCor());
        //}
        //IEnumerator ShowMainStoryCor()
        //{
        //    UI_Manager.ShowPopupUI<UI_Communication>();
        //    yield return new WaitForEndOfFrame();
        //    MainStoryParser.Inst.StartStory(ChooseMainStory());
        //}

        //public int ChooseMainStory()
        //{
        //    var HighestMainStat = Data.PlayerData.GetHigestMainStatName();

        //    return Data.PlayerData.MainStoryIndexs[(int)HighestMainStat];
        //}

        #endregion
    }
}