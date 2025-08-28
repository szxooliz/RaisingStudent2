using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class UI_EndingScene : UI_Scene, IPointerClickHandler
    {
        enum Buttons
        {
            BTN_Menu, BTN_Log, BTN_Schedule
        }

        enum Images
        {
            IMG_Background,
            IMG_Illustration,
            IMG_CharFace,
            IMG_Bubble,
            IMG_NameTag,
        }

        enum Texts
        {
            TMP_CharName, TMP_CharLine,
        }

        private string characterSpritePath = "Sprites/Character/";
        private string bgSpritePath = "Sprites/UI/BackGround/";
        private string endingSpritePath = "Sprites/UI/Ending/";

        int scriptIndex = 0; // 엔딩 대사 index
        int currentEndingNum = 0; // 현재 엔딩을 나타내는 숫자
        List<EndingScript> newEndingScripts = new List<EndingScript>(); // 현재 엔딩에 맞는 스크립트만을 저장

        private bool hasBackgroundSet = false;        // 배경화면 변경 여부 
        private bool hasIllustrationAppeared = false; // 처음 일러스트 등장 여부
        private bool isIllustrationDisplayed = false; // 현재 일러스트가 표시된 상태인지 확인

        private Coroutine coroutine = null;

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<Image>(typeof(Images));
            Bind<TMPro.TMP_Text>(typeof(Texts));

            BindButton();

            //currentEndingNum = GetLatestEndingIndex();
            currentEndingNum = (int)GameManager.Instance.endingName;
            LogManager.Instance.GetNewLogGroup("육성 완료");
            LoadScript();
            LoadIllustration();
            LoadNextScript();
            GetButton((int)Buttons.BTN_Schedule).interactable = false;
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.BTN_Menu).gameObject, OnClickMenuBtn);
            BindEvent(GetButton((int)Buttons.BTN_Log).gameObject, OnClickLogBtn);
        }

        #region 버튼 이벤트
        void OnClickMenuBtn(PointerEventData evt)
        {
            Debug.Log("메뉴 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);
            UI_Manager.Instance.ShowPopupUI<UI_MenuPopup>();
        }

        void OnClickLogBtn(PointerEventData evt)
        {
            Debug.Log("로그 버튼 클릭");
            SoundManager.Instance.Play(eSound.SFX_Positive);
            UI_Manager.Instance.ShowPopupUI<UI_LogPopup_Simple>();
        }
        #endregion

        /// <summary>
        /// 오브젝트에서 포인터를 누르고 뗄 때 호출됨
        /// </summary>
        /// <param name="evt"></param>
        public void OnPointerClick(PointerEventData evt)
        {
            LoadNextScript();
        }

        /// <summary>
        /// 엔딩에 맞는 스크립트만을 저장
        /// </summary>
        void LoadScript()
        {
            int tempIndex = 0;
            while (true)
            {
                EndingScript script = DataManager.Instance.GetData<EndingScript>(tempIndex++);

                if (script == null)
                    break;

                if (script.EndingNum == currentEndingNum) { 
                    newEndingScripts.Add(script);
                }
            }
        }

        /// <summary>
        /// 엔딩에 맞는 일러스트 로드
        /// </summary>
        void LoadIllustration()
        {
            string imagePath = endingSpritePath + $"Ending_{currentEndingNum}";
            GetImage((int)Images.IMG_Illustration).sprite = DataManager.Instance.GetOrLoadSprite(imagePath);
        }

        /// <summary>
        /// 다음 스크립트 로드
        /// </summary>
        void LoadNextScript()
        {
            if (coroutine != null)  // 텍스트 애니메이션이 진행 중이라면
            {
                coroutine = null;
                return;
            }

            //TODO: (scriptIndex == newEndingScripts.Count) 인 경우 게임 종료 나타내기
            if (scriptIndex >= newEndingScripts.Count)
            {
                SceneManager.LoadScene("TitleScene");
                return;
            }

            // 일러스트가 표시된 상태
            if (isIllustrationDisplayed)
            {
                isIllustrationDisplayed = false;
                GetImage((int)Images.IMG_Illustration).transform.SetSiblingIndex(1);  // 일러스트 인덱스 초기화
            }

            EndingScript endingScript = newEndingScripts[scriptIndex];

            // 배경 설정
            if (!hasBackgroundSet)
            {
                string bgPath = bgSpritePath + endingScript.Background;
                GetImage((int)Images.IMG_Background).sprite = DataManager.Instance.GetOrLoadSprite(bgPath);
                hasBackgroundSet = true;
            }

            // 처음으로 일러스트가 등장한 경우
            if (!hasIllustrationAppeared && endingScript.HasIllust)
            {
                LoadIllustration(endingScript);
                isIllustrationDisplayed = true;  // 일러스트가 표시되었음을 기록
                return;
            }

            // 새로운 대사 출력 시작
            coroutine = StartCoroutine(LoadNextDialogue(endingScript));

            scriptIndex++;
        }

        /// <summary>
        /// 다음 엔딩 대사 로드
        /// </summary>
        /// <param name="endingScript"></param>
        IEnumerator LoadNextDialogue(EndingScript endingScript)
        {
            eLineType eLineType = eLineType.SPEAK;

            TMPro.TMP_Text charLine = GetText((int)Texts.TMP_CharLine);
            string str = endingScript.Line;

            if (endingScript.NameTag)
            {
                GetText((int)Texts.TMP_CharName).text = Util.GetCharNameKor(endingScript.Character);
                GetImage((int)Images.IMG_NameTag).gameObject.SetActive(true);
            }
            else
            {
                GetText((int)Texts.TMP_CharName).text = "";
                GetImage((int)Images.IMG_NameTag).gameObject.SetActive(false);
            }

            string basicSprite = Util.GetCharBasicSpritePath(endingScript.Character);
            if (endingScript.Character == "Comsoon")
            {
                GetImage((int)Images.IMG_CharFace).gameObject.SetActive(true);
                string path = characterSpritePath + "Comsoon_" + endingScript.Face;
                GetImage((int)Images.IMG_CharFace).sprite = DataManager.Instance.GetOrLoadSprite(path);
            }
            else if (basicSprite != "none")
            {
                eLineType = eLineType.NARRATION;

                GetImage((int)Images.IMG_CharFace).gameObject.SetActive(true);
                string path = characterSpritePath + basicSprite;
                GetImage((int)Images.IMG_CharFace).sprite = DataManager.Instance.GetOrLoadSprite(path);
            }
            else
            {
                GetImage((int)Images.IMG_CharFace).gameObject.SetActive(false);
            }
            StartCoroutine(Util.LoadTextOneByOne(str, charLine));
            LogManager.Instance.GetLastClusterGroup().AddLine(eLineType, endingScript);

            yield return null;
        }

        /// <summary>
        /// 다음 일러스트 로드
        /// </summary>
        /// <param name="endingScript"></param>
        void LoadIllustration(EndingScript endingScript)
        {
            // 처음으로 일러스트가 등장한 경우 상태 업데이트
            if (endingScript.HasIllust)
            {
                hasIllustrationAppeared = true;
                GetImage((int)Images.IMG_Illustration).transform.SetSiblingIndex(4);
            }
        }
    }
}
