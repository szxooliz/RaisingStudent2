using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class J_UI_BaseSceneBtm : UI_Scene
    {
        enum Buttons
        {
            BTN_Rest,
            BTN_Study, BTN_Game, BTN_WorkOut, BTN_Club,
        }
        enum Texts
        {
            TMP_Inteli, TMP_Otaku, TMP_Strength, TMP_Charming,
        }
        enum UIs
        {
            MainUI, ActivityUI, EventUI
        }
        enum Images
        {
            UI_Stress, UI_StressStatus,
        }

        private string spritePath = "Sprites/UI/Stress_";
        private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        int index = 4; // 이벤트 대사 index
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
            DataManager.Instance.playerData.OnStatusChanged += OnStatusChanged;
        }

        /// <summary>
        /// 버튼 바인딩
        /// </summary>
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
            Debug.Log($"현재 상태: {DataManager.Instance.playerData.currentStatus}");

            MakeTransition((int)DataManager.Instance.playerData.currentStatus);
        }

        /// <summary>
        /// UI 트랜지션
        /// </summary>
        /// <param name="index"></param>
        public void MakeTransition(int index)
        {
            for (int i = 0; i < 3; i++)
            {
                GetGameObject(i).SetActive(false);
            }
            // 현재 상태인 UI 오브젝트만 활성화
            GetGameObject(index).SetActive(true);

            // 육성 상태일 경우 스탯, 스트레스 업데이트
            if (index == (int)UIs.MainUI)
            {
                UpdateStatUIs();
                UpdateStressUIs();
            }
        }

        /// <summary>
        /// 스탯 UI 수치 업데이트
        /// </summary>
        void UpdateStatUIs()
        {
            for (int i = 0; i < (int)eStatName.MaxCount; i++)
            {
                GetText((int)eStatName.Inteli + i).text = DataManager.Instance.playerData.statsAmounts[i].ToString();
            }
        }

        /// <summary>
        /// 스트레스 UI 바 및 상태 이미지 업데이트
        /// </summary>
        void UpdateStressUIs()
        {
            string path;

            // 상태 따라 색, 상태 이미지 정하기 
            if (DataManager.Instance.playerData.stressAmount >= 70)
            {
                GetImage((int)Images.UI_Stress).color = new Color32(255, 68, 51, 255);
                path = spritePath + "danger";
            }
            else if (DataManager.Instance.playerData.stressAmount >= 40)
            {
                GetImage((int)Images.UI_Stress).color = new Color32(243, 230, 0, 255);
                path = spritePath + "normal";
            }
            else
            {
                GetImage((int)Images.UI_Stress).color = new Color32(34, 217, 121, 255);
                path = spritePath + "calm";
            }

            // 스트레스 바 채우기
            GetImage((int)Images.UI_Stress).fillAmount = DataManager.Instance.playerData.stressAmount / 100;

            // path 경로 통해서 상태 이미지 로드
            GetImage((int)Images.UI_StressStatus).sprite = GetOrLoadSprite(path);

            // 상태 그래픽 흰색 이슈로 색상 처리
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

            StartActivity((int)eActivityType.Rest);
        }
        void OnClickStudyBtn(PointerEventData evt)
        {
            Debug.Log("공부 버튼 클릭");

            StartActivity((int)eActivityType.Class);
        }
        void OnClickGameBtn(PointerEventData evt)
        {
            Debug.Log("게임 버튼 클릭");

            StartActivity((int)eActivityType.Game);
        }
        void OnClickWorkOutBtn(PointerEventData evt)
        {
            Debug.Log("운동 버튼 클릭");

            StartActivity((int)eActivityType.Workout);
        }
        void OnClickClubBtn(PointerEventData evt)
        {
            Debug.Log("동아리 버튼 클릭");

            StartActivity((int)eActivityType.Club);
        }
        #endregion

        /// <summary>
        /// 활동 실행
        /// </summary>
        /// <param name="actType"></param>
        public void StartActivity(int actType)
        {
            GameManager.Instance.StartActivity(actType);

            // 현재 상태를 활동 상태로 변경
            DataManager.Instance.playerData.currentStatus = eStatus.Activity;

            //MakeTransition((int)UIs.ActivityUI);
        }
    }
}