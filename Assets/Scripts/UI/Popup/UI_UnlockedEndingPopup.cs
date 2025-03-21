using System.Collections;
using System.Collections.Generic;
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
            Panel, BTN_X, BTN_BigIllustraion,
        }

        enum Texts
        {
            TMP_Title, TMP_EndingName,
            TMP_Awards1, TMP_Awards2, TMP_Awards3, TMP_Awards4,
            TMP_Contents,
        }

        enum Images
        {
            IMG_Illustraion,
        }

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<TMPro.TMP_Text>(typeof(Texts));
            Bind<Image>(typeof(Images));
            BindButton();
            GetButton((int)Buttons.BTN_BigIllustraion).gameObject.SetActive(false);
            GetImage((int)Images.IMG_Illustraion).gameObject.AddComponent<Button>().onClick.AddListener(OnClickImage);
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.Panel).gameObject, OnClickPanel);
            BindEvent(GetButton((int)Buttons.BTN_X).gameObject, OnClickXBtn);
            BindEvent(GetButton((int)Buttons.BTN_BigIllustraion).gameObject, OnClickBigImage);
        }

        #region 버튼 이벤트
        void OnClickPanel(PointerEventData evt)
        {
            Debug.Log("판넬 누름..");
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        void OnClickXBtn(PointerEventData evt)
        {
            Debug.Log("x 버튼 누름");
            SoundManager.Instance.Play(eSound.SFX_Negative);
            ClosePopupUI();
        }
        void OnClickBigImage(PointerEventData evt)
        {
            Debug.Log("큰 일러스트 누름");
            SoundManager.Instance.Play(eSound.SFX_Negative);
            GetButton((int)Buttons.BTN_BigIllustraion).gameObject.SetActive(false);
        }
        void OnClickImage()
        {
            Debug.Log("이미지 누름");
            SoundManager.Instance.Play(eSound.SFX_DialogClick);
            GetButton((int)Buttons.BTN_BigIllustraion).gameObject.SetActive(true);
        }
        #endregion

        /// <summary>
        /// UnlockedEndingPopup 데이터 설정 함수
        /// </summary>
        /// <param name="ending"></param>
        public void SetUnlockedEndingPopup(Ending ending)
        {
            if (ending != null) {
                GetText((int)Texts.TMP_Title).text = "엔딩" + (char)('A' + (int)(ending.endingName));
                GetText((int)Texts.TMP_EndingName).text = GetEndingNameKor(ending.endingName);

                string imagePath = endingSpritePath + $"Ending_{(int)(ending.endingName)}";
                GetImage((int)Images.IMG_Illustraion).sprite = DataManager.Instance.GetOrLoadSprite(imagePath);
                GetButton((int)Buttons.BTN_BigIllustraion).image.sprite = DataManager.Instance.GetOrLoadSprite(imagePath);

                // 이벤트 표시
                string str = "";
                for (int i = 0; i < ending.playerData.EventRecordList.Count; i++)
                {
                    string title = ending.playerData.EventRecordList[i].Item1;
                    string record = ending.playerData.EventRecordList[i].Item2;
                    switch (title)
                    {
                        case "1학기 중간":
                            GetText((int)Texts.TMP_Awards1).text = record;
                            break;
                        case "1학기 기말":
                            GetText((int)Texts.TMP_Awards2).text = record;
                            break;
                        case "2학기 중간":
                            GetText((int)Texts.TMP_Awards3).text = record;
                            break;
                        case "2학기 기말":
                            GetText((int)Texts.TMP_Awards4).text = record;
                            break;
                        default:
                            str += title + " " + record + "\n";
                            break;
                    }
                }
                GetText((int)Texts.TMP_Contents).text = str;
            }

        }
    }
}

