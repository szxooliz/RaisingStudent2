using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.Define;

namespace Client
{
    public class UI_BaseScene : UI_Scene
    {
        enum Buttons
        {
            BTN_Menu, BTN_Schedule, BTN_Log
        }

        enum Texts
        {
            TXT_Turn, TXT_Term
        }
        enum Images
        {
            IMG_Status
        }

        private string spritePath = "Sprites/UI/Status_";
        private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<TMPro.TMP_Text>(typeof(Texts));
            Bind<Image>(typeof(Images));

            BindButton();

            UpdateTermUI();
            UpdateTurnUI();

            DataManager.Instance.playerData.OnStatusChanged += OnStatusChanged;
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.BTN_Menu).gameObject, OnClickMenuBtn);
            BindEvent(GetButton((int)Buttons.BTN_Schedule).gameObject, OnClickScheduleBtn);
            BindEvent(GetButton((int)Buttons.BTN_Log).gameObject, OnClickLogBtn);
        }

        #region 버튼 이벤트
        void OnClickMenuBtn(PointerEventData evt)
        {
            Debug.Log("메뉴 버튼 클릭");
            UI_Manager.Instance.ShowPopupUI<UI_MenuPopup>();
        }

        void OnClickScheduleBtn(PointerEventData evt)
        {
            Debug.Log("학사 일정 버튼 클릭");
            UI_Manager.Instance.ShowPopupUI<UI_SchedulePopup>();
        }

        void OnClickLogBtn(PointerEventData evt)
        {
            Debug.Log("로그 버튼 클릭");
            UI_Manager.Instance.ShowPopupUI<UI_LogPopup>();
        }

        #endregion

        void OnStatusChanged(object sender, System.EventArgs e)
        {
            string path = "";

            switch(DataManager.Instance.playerData.currentStatus)
            {
                case Status.Main:
                    UpdateTermUI();
                    UpdateTurnUI();
                    path = spritePath + Status.Main.ToString();
                    break;
                case Status.Activity:
                    // TODO : 로그 버튼 비활성화 - 반투명 이미지 위에 붙여서 활성화
                    path = spritePath + Status.Activity.ToString();
                    break;
                case Status.Event:
                    path = spritePath + Status.Event.ToString();
                    break;
            }

            GetImage((int)Images.IMG_Status).sprite = GetOrLoadSprite(path);
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


        /// <summary>
        /// 학사 일정 표시된 주요 이벤트까지 남은 턴 표시
        /// </summary>
        void UpdateTurnUI()
        {
            int turn = DataManager.Instance.playerData.currentTurn;

            for (int i = 0; i < UI_SchedulePopup.scheduledTurn.Length; i++)
            {
                if (turn == UI_SchedulePopup.scheduledTurn[i])
                {
                    GetText((int)Texts.TXT_Turn).text = UI_SchedulePopup.scheduleTitle[i]; // scheduleText에 내용이 있으면 바꿔주고 종료
                    return;
                }
                // TODO : 다음 스케줄에 맞춰 남은 턴 표시 로직 더 추가
                GetText((int)Texts.TXT_Turn).text = "앞으로 " + (UI_SchedulePopup.scheduledTurn[i + 1] - turn) + "턴";
            }
        }

        /// <summary>
        /// 시기 표시 UI 업데이트
        /// </summary>
        void UpdateTermUI()
        {
            GetText((int)Texts.TXT_Term).text = (int)DataManager.Instance.playerData.currentMonth + "월 " 
                                                + Define.GetThirdsKor(DataManager.Instance.playerData.currentThird);
        }
    }

}
