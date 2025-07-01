using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class UI_TitleScene : UI_Scene
    {
        enum Buttons
        {
            BTN_NewGame, BTN_Continue, BTN_EndingList, BTN_Settings,
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));

            BindButton();
        }

        /// <summary>
        /// 버튼 바인딩
        /// </summary>
        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.BTN_NewGame).gameObject, OnClickNewGameBtn);
            BindEvent(GetButton((int)Buttons.BTN_Continue).gameObject, OnClickContinueBtn);
            BindEvent(GetButton((int)Buttons.BTN_EndingList).gameObject, OnClickEndingListBtn);
            BindEvent(GetButton((int)Buttons.BTN_Settings).gameObject, OnClickSettingsBtn);
        }

        #region Title 버튼 이벤트
        void OnClickNewGameBtn(PointerEventData evt)
        {
            Debug.Log("새로 하기 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);

            // 세이브 데이터 확인 후 팝업 띄우기
            if (IsExistSaveDatas())
            {
                UI_Manager.Instance.ShowPopupUI<UI_NewGamePopup>();
            }
            else
            {
                DataManager.Instance.LoadPlayerData();
                SceneManager.LoadScene("BaseScene");
            }
        }
        void OnClickContinueBtn(PointerEventData evt)
        {
            Debug.Log("이어 하기 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);

            // 세이브 데이터 확인 후 이동
            if (!IsExistSaveDatas())
            {
                Debug.LogError("세이브 데이터 없음. 새로하기 ㄱㄱ");
            }
            else
            {
                SceneManager.LoadScene("BaseScene");
            }
        }
        void OnClickEndingListBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);
            SceneManager.LoadScene("EndingListScene");
        }
        void OnClickSettingsBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);
            UI_Manager.Instance.ShowPopupUI<UI_SettingsPopup>();
        }
        #endregion

        bool IsExistSaveDatas()
        {
            string playerData_path = $"{Application.persistentDataPath}/PlayerData.json";

            return File.Exists(playerData_path);
        }

    }

}
