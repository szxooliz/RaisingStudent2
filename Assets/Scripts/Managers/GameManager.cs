using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using static Client.SystemEnum;

namespace Client
{
    public class GameManager : MonoBehaviour
    {
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
            
            DataManager.Instance.SetNewActivityData((eActivityType)actType);

            // 자체 휴강일 때는 업데이트할 스탯 없음
            if (actType != (int)eActivityType.Rest) 
            { 
                UpdateStats(); 
            }

            UpdateStress();

            // 활동마다 데이터 저장 - TODO : 출시 때 주석 해제
            //Data.SavePlayerData();
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
            DataManager.Instance.activityData.resultType = GetResult();
            float multiplier = GetStatMultiplier(DataManager.Instance.activityData.resultType);

            for (int i = 0; i < DataManager.Instance.activityData.statNames.Count; i++)
            {
                // 소수점 올림 처리
                int value = (int)Math.Ceiling(DataManager.Instance.activityData.statValues[i] * multiplier);
                DataManager.Instance.activityData.statValues[i] = value;

                // 스탯 증감 처리
                DataManager.Instance.playerData.StatsAmounts[(int)DataManager.Instance.activityData.statNames[i]] += value;
            }
        }

        /// <summary>
        /// 스트레스 증감 함수
        /// </summary>
        /// <param name="amount">음수: 감소</param>
        public void UpdateStress()
        {
            DataManager.Instance.playerData.StressAmount += DataManager.Instance.activityData.stressValue;
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
                // TODO : 6월과 9월 사이 여름방학 달 추가해야 하므로 수정 필요
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