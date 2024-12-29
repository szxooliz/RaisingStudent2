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

        // ResourceManager _resource = new ResourceManager();
        //UI_Manager _ui_manager = new UI_Manager();
        //SoundManager _sound = new SoundManager();
        DataManager _data = DataManager.Instance;
        EventManager _event = EventManager.Instance;

        ////REventManager _RE = new REventManager();
        // public static ResourceManager Resource { get { return Instance._resource; } }
        //public static UI_Manager UI_Manager { get { return Instance._ui_manager; } }
        //public static SoundManager Sound { get { return Instance._sound; } }
        public static DataManager Data { get { return Instance._data; } }
        public static EventManager Event { get { return Instance._event; } }


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

                //s_instance._sound.Init();
                Data.Init();
                //s_instance._scene.Init();
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
            
            Data.SetNewActivityData((ActivityType)actType);


            // 자체 휴강일때는 업데이트할 스탯 없음
            if (actType != (int)ActivityType.Rest) 
            { 
                UpdateStats(); 
            }

            UpdateStress();
            NextTurn();
        }

        /// <summary>
        /// 스트레스에 따른 활동 결과 리턴
        /// </summary>
        /// <returns></returns>
        public ResultType GetResult()
        {
            int prob = UnityEngine.Random.Range(0, 100);

            if (Data.playerData.stressAmount >= 70)
            {
                if (prob <= 50) { return ResultType.Failure; }
                else if (prob <= 95) { return ResultType.Success; }
                else { return ResultType.BigSuccess; }
            }
            else if (Data.playerData.stressAmount >= 40 && Data.playerData.stressAmount < 70)
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
            Data.activityData.resultType = GetResult();
            float multiplier = GetStatMultiplier(Data.activityData.resultType);

            Debug.Log("활동 결과 : " + GetResultTypeKor(Data.activityData.resultType));

            // 소수점 올림 처리
            Data.activityData.stat1Value = (int)Math.Ceiling(Data.activityData.stat1Value * multiplier);
            Data.activityData.stat2Value = (int)Math.Ceiling(Data.activityData.stat2Value * multiplier);

            Data.playerData.statsAmounts[(int)Data.activityData.statName1] += Data.activityData.stat1Value;
            Data.playerData.statsAmounts[(int)Data.activityData.statName2] += Data.activityData.stat2Value;

            Debug.Log(GetStatNameKor(Data.activityData.statName1) + " : " + Data.playerData.statsAmounts[(int)Data.activityData.statName1]);
            Debug.Log(GetStatNameKor(Data.activityData.statName2) + " : " + Data.playerData.statsAmounts[(int)Data.activityData.statName2]);
        }

        /// <summary>
        /// 스트레스 증감 함수
        /// </summary>
        /// <param name="amount">음수: 감소</param>
        public void UpdateStress()
        {
            Data.playerData.stressAmount += Data.activityData.stressValue;
            Debug.Log("스트레스 : " + DataManager.Instance.playerData.stressAmount);
            // -> 바텀씬에서 mainUI 재활성화할 때마다 플레이어데이터.스트레스 보고 슬라이드 바 업데이트하도록
        }

        public void NextTurn()
        {
            Data.playerData.currentTurn++;

            Data.playerData.currentThird = (Thirds)(Data.playerData.currentTurn % 3);

            if (Data.playerData.currentThird == 0)
            {
                Data.playerData.currentMonth++;
            }
            Debug.Log(Data.playerData.currentMonth + "월 " + Data.playerData.currentThird + "순");
        }

        // 5. 목표 스케줄까지 남은 턴 계산


        // 7. 마지막 턴 이후에 스탯 계산해서 엔딩 결과 내기

        // 8. 엔딩 보여주기 ShowEnding





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
        //public void ShowReceipt()
        //{
        //    StartCoroutine(ShowReceiptCor());
        //}
        //IEnumerator ShowReceiptCor()
        //{
        //    if (UI_Tutorial.instance == null)
        //        UI_Manager.CloseALlPopupUI();

        //    yield return new WaitForEndOfFrame();
        //    UI_Manager.ShowPopupUI<UI_Reciept>();
        //}

        //public void ShowSelectNickName()
        //{
        //    StartCoroutine(ShowSelectNickNameCor());
        //}
        //IEnumerator ShowSelectNickNameCor()
        //{
        //    UI_Manager.ClosePopupUI();
        //    yield return new WaitForEndOfFrame();
        //    UI_Manager.ShowPopupUI<UI_SelectNickName>();
        //}

        //public void CloseTitle()
        //{
        //    if (GameManager.Data.PersistentUser.WatchedTutorial == false)
        //        StartCoroutine(ShowTutorialCor());
        //    else
        //        GameManager.UI_Manager.ClosePopupUI();
        //}
        //IEnumerator ShowTutorialCor()
        //{
        //    UI_Manager.CloseALlPopupUI();
        //    yield return new WaitForEndOfFrame();
        //    UI_Manager.ShowPopupUI<UI_Tutorial>();
        //}

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