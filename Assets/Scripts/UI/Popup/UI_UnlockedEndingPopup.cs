using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class UI_UnlockedEndingPopup : UI_Popup
    {
        private string endingSpritePath = "Sprites/UI/Ending/";

        enum Buttons
        {
            Panel, BTN_X, BTN_BigIllustraion, BTN_Enlarge, BTN_Illustration
        }

        enum Texts
        {
            TMP_Title, TMP_EndingName, TMP_Contents,
            TMP_Awards1, TMP_Awards2, TMP_Awards3, TMP_Awards4,
        }

        enum GameObjects
        {
            Maxs, MaskArea
        }
        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<TMPro.TMP_Text>(typeof(Texts));
            Bind<GameObject>(typeof(GameObjects));

            BindButton();

            GetGameObject((int)GameObjects.Maxs).gameObject.SetActive(false);
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_X).gameObject, OnClickXBtn);
            BindEvent(GetButton((int)Buttons.BTN_BigIllustraion).gameObject, OnClickBigImage);

            BindEvent(GetButton((int)Buttons.BTN_Enlarge).gameObject, OnClickImage);
            BindEvent(GetButton((int)Buttons.BTN_Illustration).gameObject, OnClickImage);
        }

        #region 버튼 이벤트
        void OnClickPanel(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        void OnClickXBtn(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        void OnClickBigImage(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_Negative);
            GetGameObject((int)GameObjects.Maxs).SetActive(false);
        }
        void OnClickImage(PointerEventData evt)
        {
            SoundManager.Instance.Play(eSound.SFX_DialogClick);
            GetGameObject((int)GameObjects.Maxs).SetActive(true);
        }
        #endregion

        public void SetUnlockedEndingPopup(Ending ending)
        {
            if (ending == null)
                return;

            // 엔딩 제목 설정
            GetText((int)Texts.TMP_Title).text = $"엔딩{(char)('A' + (int)(ending.EndingName))}";
            GetText((int)Texts.TMP_EndingName).text = GetEndingNameKor(ending.EndingName);

            // 일러스트 설정
            string imagePath = $"{endingSpritePath}Ending_{(int)(ending.EndingName)}";
            GetButton((int)Buttons.BTN_Illustration).image.sprite = DataManager.Instance.GetOrLoadSprite(imagePath);
            GetButton((int)Buttons.BTN_BigIllustraion).image.sprite = DataManager.Instance.GetOrLoadSprite(imagePath);

            // 시험 성적 입력
            for (int i = 0; i < ending.playerData.EventRecordList.Count; i++)
            {
                GetText((int)Texts.TMP_Awards1 + i).text = ending.playerData.EventRecordList[i].Record;
            }

            // 기타 이력 입력
            StringBuilder sb = new();
            foreach(var recordList in ending.playerData.EventRecordList_etc)
            {
                sb.AppendLine($"{recordList.Title} {recordList.Record}");
            }
            GetText((int)Texts.TMP_Contents).text = sb.ToString();

        }
    }
}

