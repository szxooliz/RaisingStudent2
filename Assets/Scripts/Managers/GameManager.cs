using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Client
{
    public class GameManager : MonoBehaviour
    {
        //[SerializeField] private int _mainStatUpValuePerTier;
        //[SerializeField] private float _str_MenDownValuePerTier;
        //[SerializeField] private int _bigSuccessProbabilityPerTier;
        //[SerializeField] private float _bigSuccessCoefficientValuePerTier;

        //public int MainStat_ValuePerLevel => _mainStatUpValuePerTier;
        //public float Str_Men_ValuePerLevel => _str_MenDownValuePerTier;
        //public int BigSuccessProbability => _bigSuccessProbabilityPerTier;
        //public float BigSuccessCoefficientValue => _bigSuccessCoefficientValuePerTier;

        static GameManager s_instance;
        public static GameManager Instance { get { Init(); return s_instance; } }
        GameManager() { }

        // ResourceManager _resource = new ResourceManager();
        //UI_Manager _ui_manager = new UI_Manager();
        //SoundManager _sound = new SoundManager();
        //DataManager _data = new DataManager();


        ////REventManager _RE = new REventManager();
        // public static ResourceManager Resource { get { return Instance._resource; } }
        //public static UI_Manager UI_Manager { get { return Instance._ui_manager; } }
        //public static SoundManager Sound { get { return Instance._sound; } }
        //public static DataManager Data { get { return Instance._data; } }


        void Awake()
        {
            Init();
            //StartCoroutine(LoadDatas());
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
                DataManager.Instance.Init();
                //s_instance._scene.Init();
            }
        }

        //IEnumerator LoadDatas()
        //{
        //    Coroutine cor1 = StartCoroutine(s_instance._data.RequestAndSetDayDatas(DayDatasURL));
        //    Coroutine cor2 = StartCoroutine(s_instance._data.RequestAndSetRandEventDatas(RandEventURL));
        //    Coroutine cor3 = StartCoroutine(s_instance._data.RequestAndSetItemDatas(MerchantURL));

        //    yield return cor1;
        //    yield return cor2;
        //    yield return cor3;

        //}


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