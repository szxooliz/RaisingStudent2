using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{ 
    public class UI_BaseSceneBtm : UI_Scene
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
            UI_Stress, UI_StressStatus, IMG_Background
        }

        Conversation conversation;

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

            conversation = gameObject.GetComponentInChildren<Conversation>();
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
        void OnStatusChanged(object sender, System.EventArgs e)
        {
            MakeTransition((int)DataManager.Instance.playerData.CurrentStatus);
        }

        /// <summary>
        /// UI 트랜지션
        /// </summary>
        /// <param name="index"></param>
        public void MakeTransition(int index)
        {
            // 메인은 남겨두고 이벤트&활동만 껐다 켰다 해야겠다

            for (int i = 0; i < 3; i++)
            {
                GetGameObject(i).SetActive(false);
            }

            // 현재 상태인 UI 오브젝트만 활성화
            GetGameObject(index).SetActive(true);

            // 육성 상태일 경우 스탯, 스트레스 업데이트
            if (index == (int)UIs.MainUI)
            {
                ChangeBackGround("bg_school");
                UpdateStatUIs();
                UpdateStressUIs();
                StartCoroutine(conversation.ResetBubble());
            }
        }

        #region 스탯, 스트레스 UI 업데이트
        /// <summary>
        /// 스탯 UI 수치 업데이트
        /// </summary>
        void UpdateStatUIs()
        {
            for (int i = 0; i < (int)eStatName.MaxCount; i++)
            {
                GetText((int)eStatName.Inteli + i).text = DataManager.Instance.playerData.StatsAmounts[i].ToString();
            }
        }

        /// <summary>
        /// 스트레스 UI 바 및 상태 이미지 업데이트
        /// </summary>
        void UpdateStressUIs()
        {
            string status;
            // 상태 따라 색, 상태 이미지 정하기 
            if (DataManager.Instance.playerData.StressAmount >= 70)
            {
                GetImage((int)Images.UI_Stress).color = new Color32(255, 68, 51, 255);
                status = "danger";
            }
            else if(DataManager.Instance.playerData.StressAmount >= 40)
            {
                GetImage((int)Images.UI_Stress).color = new Color32(243, 230, 0, 255);
                status = "normal";
            }
            else
            {
                GetImage((int)Images.UI_Stress).color = new Color32(34, 217, 121, 255);
                status = "calm";
            }

            string path = $"Sprites/UI/Stress/Stress_{status}";

            // 스트레스 바 채우기
            GetImage((int)Images.UI_Stress).fillAmount = DataManager.Instance.playerData.StressAmount / 100;
            
            // path 경로 통해서 상태 이미지 로드
            GetImage((int)Images.UI_StressStatus).sprite = DataManager.Instance.GetOrLoadSprite(path);

            // 상태 그래픽 흰색 이슈로 색상 처리
            GetImage((int)Images.UI_StressStatus).color = Color.black; 
        }

        #endregion

        #region MainUI 버튼 이벤트
        void OnClickRestBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);
            StartActivity(eActivityType.Rest);
            ChangeBackGround("bg_classroom");
        }
        void OnClickStudyBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);
            StartActivity(eActivityType.Class);
            ChangeBackGround("bg_classroom");
        }
        void OnClickGameBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);
            StartActivity(eActivityType.Game);
            ChangeBackGround("bg_home");
        }
        void OnClickWorkOutBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);
            StartActivity(eActivityType.Workout);
            ChangeBackGround("bg_gym");
        }
        void OnClickClubBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);
            StartActivity(eActivityType.Club);
            ChangeBackGround("bg_club");
        }
        #endregion

        /// <summary>
        /// 활동 실행
        /// </summary>
        /// <param name="actType"></param>
        public void StartActivity(eActivityType actType)
        {
            GameManager.Instance.StartActivity(actType);

            // 현재 상태를 활동 상태로 변경
            DataManager.Instance.playerData.CurrentStatus = eStatus.Activity;
        }

        void ChangeBackGround(string spritePath)
        {
            if (spritePath == null) return;

            string path = $"Sprites/UI/BackGround/{spritePath}";
            GetImage((int)Images.IMG_Background).sprite = DataManager.Instance.GetOrLoadSprite(path);
        }

    }
}