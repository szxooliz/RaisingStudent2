using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class J_UI_BaseScene : UI_Scene
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

            switch (DataManager.Instance.playerData.CurrentStatus)
            {
                case eStatus.Main:
                    UpdateTermUI();
                    UpdateTurnUI();
                    path = spritePath + eStatus.Main.ToString();
                    break;
                case eStatus.Activity:
                    path = spritePath + eStatus.Activity.ToString();
                    break;
                case eStatus.Event:
                    path = spritePath + eStatus.Event.ToString();
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


        // TODO : 주요 이벤트까지 남은 턴 표시 - 이벤트 상세 기획서 나오면 
        void UpdateTurnUI()
        {

        }

        /// <summary>
        /// 시기 표시 UI 업데이트
        /// </summary>
        void UpdateTermUI()
        {
            GetText((int)Texts.TXT_Term).text = (int)DataManager.Instance.playerData.CurrentMonth + "월 "
                                                + SystemEnum.GetThirdsKor(DataManager.Instance.playerData.CurrentThird);
        }
    }

}
