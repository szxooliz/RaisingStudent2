using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.Define;

namespace Client
{ 
    public class UI_BaseSceneBtm : UI_Scene
    {
        enum Buttons
        {
            BTN_Rest,
            BTN_Study, BTN_Game, BTN_WorkOut, BTN_Club,
            // MaxCount
        }
        enum Texts
        {
            // 스탯 수치
            TMP_Inteli, TMP_Otaku, TMP_Strength, TMP_Charming,

            //MaxCount
        }
        enum UIs
        {
            MainUI, ActivityUI, EventUI
        }
        enum Images
        {
            UI_Stress, UI_StressStatus
        }

        private string spritePath = "Sprites/UI/Stress_";
        private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();


        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<TMPro.TMP_Text>(typeof(Texts));
            Bind<GameObject>(typeof(UIs));
            Bind<Image>(typeof(Images));

            BindButton();

            GetGameObject((int)UIs.MainUI).SetActive(true);
            GetGameObject((int)UIs.ActivityUI).SetActive(false);
            GetGameObject((int)UIs.EventUI).SetActive(false);

            UpdateStatUIs();
            UpdateStressUIs();
            GameManager.Data.playerData.OnStatusChanged += OnStatusChanged;
        }


        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.BTN_Rest).gameObject, OnClickRestBtn);
            BindEvent(GetButton((int)Buttons.BTN_Study).gameObject, OnClickStudyBtn);
            BindEvent(GetButton((int)Buttons.BTN_Game).gameObject, OnClickGameBtn);
            BindEvent(GetButton((int)Buttons.BTN_WorkOut).gameObject, OnClickWorkOutBtn);
            BindEvent(GetButton((int)Buttons.BTN_Club).gameObject, OnClickClubBtn);

        }

        /// <summary>
        /// 현재 상태에 맞추어 UI 트랜지션
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnStatusChanged(object sender, System.EventArgs e)
        {
            MakeTransition((int)GameManager.Data.playerData.currentStatus);
        }
        public void MakeTransition(int index)
        {
            Debug.Log($"Transition {(Status)index}");

            for (int i = 0; i < 3; i++)
            {
                GetGameObject(i).SetActive(false);
            }

            GetGameObject(index).SetActive(true);

            if (index == (int)UIs.MainUI)
            {
                Debug.Log("메인 UI 업데이트");
                UpdateStatUIs();
                UpdateStressUIs();
            }
        }
        void UpdateStatUIs()
        {
            for (int i = 0; i < (int)StatName.MaxCount; i++)
            {
                GetText((int)StatName.Inteli + i).text = GameManager.Data.playerData.statsAmounts[i].ToString();
            }
        }
        void UpdateStressUIs()
        {
            string path;

            // 상태 따라 색, 상태 이미지 정하기 
            if (GameManager.Data.playerData.stressAmount >= 70)
            {
                GetImage((int)Images.UI_Stress).color = new Color32(255, 68, 51, 255);
                path = spritePath + "danger";
            }
            else if(GameManager.Data.playerData.stressAmount >= 40)
            {
                GetImage((int)Images.UI_Stress).color = new Color32(243, 230, 0, 255);
                path = spritePath + "normal";
            }
            else
            {
                GetImage((int)Images.UI_Stress).color = new Color32(34, 217, 121, 255);
                path = spritePath + "calm";
            }

            GetImage((int)Images.UI_Stress).fillAmount = GameManager.Data.playerData.stressAmount / 100;
            GetImage((int)Images.UI_StressStatus).sprite = GetOrLoadSprite(path);
            GetImage((int)Images.UI_StressStatus).color = Color.black;
        }

        Sprite GetOrLoadSprite(string _path)
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

        #region MainUI 버튼 이벤트
        void OnClickRestBtn(PointerEventData evt)
        {
            Debug.Log("자체 휴강 버튼 클릭");

            StartActivity((int)ActivityType.Rest);
        }
        void OnClickStudyBtn(PointerEventData evt)
        {
            Debug.Log("공부 버튼 클릭");

            StartActivity((int)ActivityType.Class);
        }
        void OnClickGameBtn(PointerEventData evt)
        {
            Debug.Log("게임 버튼 클릭"); 

            StartActivity((int)ActivityType.Game);
        }
        void OnClickWorkOutBtn(PointerEventData evt)
        {
            Debug.Log("운동 버튼 클릭");

            StartActivity((int)ActivityType.Workout);
        }
        void OnClickClubBtn(PointerEventData evt)
        {
            Debug.Log("동아리 버튼 클릭");

            StartActivity((int)ActivityType.Club);
        }
        #endregion

        public void StartActivity(int actType)
        {
            GameManager.Instance.StartActivity(actType);
            GameManager.Data.playerData.currentStatus = Status.Activity;

            MakeTransition((int)UIs.ActivityUI);
        }


    }
}