using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            IMG_Background, IMG_Illustration, IMG_CharFace, IMG_Bubble, IMG_NameTag,
        }

        enum Texts
        {
            TMP_CharName, TMP_CharLine,
        }

        private string characterSpritePath = "Sprites/Character/";
        private string bgSpritePath = "Sprites/UI/BackGround/";
        private string endingSpritePath = "Sprites/UI/Ending/";

        long scriptIndex = 0; // 엔딩 대사 index
        int currentEndingNum = 0; // 현재 엔딩을 나타내는 숫자
        Dictionary<long, EndingScript> endingScriptDict = new(); // 현재 엔딩에 맞는 스크립트만을 저장
        EndingScript pastEndingScript = null;

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

            LoadEndingData();
            coroutine = StartCoroutine(LoadNextDialogue());

            GetButton((int)Buttons.BTN_Schedule).interactable = false;
            LogManager.Instance.GetNewClusterGroup("육성 완료");

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


        /// <summary> 오브젝트에서 포인터를 누르고 뗄 때 호출됨 </summary>
        public void OnPointerClick(PointerEventData evt)
        {
            // 텍스트 입력 중(또는 첫 번째 클릭 후 전체 노출 대기 중)이라면 무시
            if (Util.nowTexting)
                return;

            SoundManager.Instance.Play(eSound.SFX_DialogClick);
            coroutine = StartCoroutine(LoadNextDialogue());
        }
        /// <summary> 필요한 데이터 미리 로드 </summary>
        void LoadEndingData()
        {
            currentEndingNum = (int)GameManager.Instance.endingName;
            endingScriptDict = DataManager.Instance.EndingScriptDict[currentEndingNum];
            scriptIndex = endingScriptDict.Keys.Min();
            LoadIllustration();

            DataManager.Instance.SaveAllData();
        }


        /// <summary> 엔딩에 맞는 일러스트 로드 </summary>
        void LoadIllustration()
        {
            string imagePath = endingSpritePath + $"Ending_{currentEndingNum}";
            GetImage((int)Images.IMG_Illustration).sprite = DataManager.Instance.GetOrLoadSprite(imagePath);
        }

        EndingScript GetNextScript(long index)
        {
            return endingScriptDict.GetValueOrDefault(index, null);
        }



        /// <summary> 다음 엔딩 대사 로드 </summary>
        IEnumerator LoadNextDialogue()
        {
            // 일러스트가 표시된 상태
            if (isIllustrationDisplayed)
            {
                isIllustrationDisplayed = false;
                GetImage((int)Images.IMG_Illustration).transform.SetSiblingIndex(1);  // 일러스트 인덱스 초기화
            }

            EndingScript endingScript = GetNextScript(scriptIndex);
            DisplayScript(endingScript);
            pastEndingScript = endingScript;

            // 처음으로 일러스트가 등장한 경우
            if (!hasIllustrationAppeared && endingScript.HasIllust)
            {
                LoadIllustration(endingScript);
                isIllustrationDisplayed = true;  // 일러스트가 표시되었음을 기록
            }

            yield return null;

        }

        void DisplayScript(EndingScript endingScript)
        {
            if (endingScript == null)
            {
                StartCoroutine(DelayedTransition());
                SceneManager.LoadScene("TitleScene");
                return;
            }

            UpdateScriptUI(endingScript);
            StartCoroutine(Util.LoadTextOneByOne(endingScript.Line, GetText((int)Texts.TMP_CharLine)));
            scriptIndex++;
        }

        /// <summary> 해당 스크립트 관련 UI 세팅 </summary>
        void UpdateScriptUI(EndingScript endingScript)
        {
            // 배경 설정
            if (!hasBackgroundSet)
            {
                string bgPath = bgSpritePath + endingScript.Background;
                GetImage((int)Images.IMG_Background).sprite = DataManager.Instance.GetOrLoadSprite(bgPath);
                hasBackgroundSet = true;
            }

            eLineType eLineType = eLineType.SPEAK;

            TMPro.TMP_Text charLine = GetText((int)Texts.TMP_CharLine);
            string str = endingScript.Line;

            GetImage((int)Images.IMG_NameTag).gameObject.SetActive(endingScript.NameTag);
            GetText((int)Texts.TMP_CharName).text = endingScript.NameTag ? 
                Util.GetCharNameKor(endingScript.Character) : "";


            string basicSprite = Util.GetCharBasicSpritePath(endingScript.Character);
            if (endingScript.Character == "Comsoon")
            {
                GetImage((int)Images.IMG_CharFace).gameObject.SetActive(true);
                string path = characterSpritePath + "Comsoon_" + endingScript.Face;
                GetImage((int)Images.IMG_CharFace).sprite = DataManager.Instance.GetOrLoadSprite(path);
            }
            else if (basicSprite == "none") // 냐레이션, 플레이어이면 표시 안함
            {
                eLineType = eLineType.NARRATION;

                GetImage((int)Images.IMG_CharFace).gameObject.SetActive(false);
            }
            else
            {
                // 컴순, 나레이션 제외라면 이미지 표시해야 됨
                GetImage((int)Images.IMG_CharFace).gameObject.SetActive(true);
                string path = characterSpritePath + basicSprite;
                GetImage((int)Images.IMG_CharFace).sprite = DataManager.Instance.GetOrLoadSprite(path);

            }

            StartCoroutine(Util.LoadTextOneByOne(str, charLine));
            LogManager.Instance.GetLastClusterGroup().AddLine(eLineType, endingScript);

        }

        /// <summary> 다음 일러스트 로드 </summary>
        void LoadIllustration(EndingScript endingScript)
        {
            // 처음으로 일러스트가 등장한 경우 상태 업데이트
            if (endingScript.HasIllust)
            {
                hasIllustrationAppeared = true;
                GetImage((int)Images.IMG_Illustration).transform.SetSiblingIndex(4);
            }
        }
        IEnumerator DelayedTransition()
        {
            yield return new WaitForSeconds(0.3f); // 최종 스크립트 표시 시간 확보
        }

    }
}
