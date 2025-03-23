using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class ActivityUI : UI_Base, IPointerClickHandler
    {
        #region 기획 조정용
        [SerializeField] float charDuration = 5f;
        [SerializeField] float narDuration = 5f;

        private List<string> bigSuccessLines = new List<string>()
        {
            "오늘 출결 시스템이 고장났대요! 나이스 타이밍!",
            "오늘 집중이 너무 잘 됐어요! 교수님의 깔깔 유머집 시리즈까지 전부 기억나요!",
            "22킬 0데스 7어시! 게임이 너무 쉬운데요!",
            "백만 스물하나… 백만 스물둘…! 하루 종일 할 수도 있어요!",
            "동방에서 피자 먹기로 했어요! 내일 봬요!"
        };
        private List<string> successLines = new List<string>()
        {
            "몇 번까지는 빠져도 점수 안 깎이니까!",
            "수업 끝! 집 가서 복습만 하면 되겠는데요!",
            "8킬 3데스 13어시, 이 정도 쯤이야!",
            "목표보다 십 분 더 뛰었어요… 체력이 늘었나?!",
            "동방에서 낮잠 자야지…!"
        };
        private List<string> failLines = new List<string>()
        {
            "어떡해, 교수님이 오늘 출석 두 번이나 부르셨대요…!",
            "하아아암… 엇, 벌써 수업 끝인가?!",
            "0킬 9데스 2어시…… 게임 접을까?",
            "으으으… 힘들어… 단백질 쉐이크 맛 없어…",
            "헉… 동방에 있는 에너지 드링크, 먹으면 안 되는 거였다고요?!"
        };
        #endregion

        #region enum
        enum GameObjects
        {
            Activity1, Activity2, Stats
        }

        enum Texts
        {
            // 스탯 수치
            TMP_Inteli, TMP_Otaku, TMP_Strength, TMP_Charming,

            // 대사 및 나레이션
            TMP_CharName, TMP_CharLine, TMP_Line
        }

        enum Images
        {
            IMG_CharFace,
            IMG_Bubble1, IMG_Bubble2
        }
        #endregion

        private Image          charFace; // 캐릭터 이미지
        private TMPro.TMP_Text charName; // 캐릭터 이름표
        private TMPro.TMP_Text charLine; // 캐릭터 대사
        private TMPro.TMP_Text line;     // 결과 나레이션

        private Coroutine coroutine = null;

        public override void Init()
        {
            Bind<GameObject>(typeof(GameObjects));
            Bind<TMPro.TMP_Text>(typeof(Texts));
            Bind<Image>(typeof(Images));
        }

        private void OnEnable()
        {
            charFace = GetImage((int)Images.IMG_CharFace);
            charName = GetText((int)Texts.TMP_CharName);
            charLine = GetText((int)Texts.TMP_CharLine);
            line = GetText((int)Texts.TMP_Line);

            GetGameObject((int)GameObjects.Activity1).SetActive(true);
            GetGameObject((int)GameObjects.Activity2).SetActive(false);

            if (DataManager.Instance.playerData.CurrentStatus == eStatus.Activity)
            {
                coroutine = StartCoroutine(ShowResult1());
                charName.text = Util.GetCharNameKor(DataManager.Instance.playerData.CharName);
                UpdateStatUIs();
            }
        }

        private void Start()
        {
            if (DataManager.Instance.playerData.CurrentStatus != eStatus.Activity) return;

            GetGameObject((int)GameObjects.Activity1).SetActive(true);
            GetGameObject((int)GameObjects.Activity2).SetActive(false);
        }

        /// <summary>
        /// 오브젝트에서 포인터를 누르고 뗄 때 호출됨
        /// </summary>
        /// <param name="evt"></param>
        public void OnPointerClick(PointerEventData evt)
        {
            if (coroutine != null) // 타이핑 애니메이션 중인 경우에
            {
                StopCoroutine(coroutine);
                coroutine = null;

                // 텍스트 전체 보여주기
                if (GetGameObject((int)GameObjects.Activity1).activeSelf)
                {
                    charLine.maxVisibleCharacters = charLine.text.Length;
                    charLine.ForceMeshUpdate();
                }
                else if (GetGameObject((int)GameObjects.Activity2).activeSelf)
                {
                    line.maxVisibleCharacters = line.text.Length;
                }
            }
            else
            {
                SoundManager.Instance.Play(eSound.SFX_DialogClick);

                // 다음 단계로 넘어가기
                if (GetGameObject((int)GameObjects.Activity1).activeSelf)
                {
                    GetGameObject((int)GameObjects.Activity2).SetActive(true);
                    GetGameObject((int)GameObjects.Activity1).SetActive(false);

                    coroutine = StartCoroutine(ShowResult2());
                }
                else // 메인으로 돌아가기
                {
                    GameManager.Instance.NextTurn();
                    EventManager.Instance.CheckEvent();
                }
            }
        }

        /// <summary>
        /// 스탯 UI를 업데이트
        /// </summary>
        void UpdateStatUIs()
        {
            for (int i = 0; i < (int)eStatName.MaxCount; i++)
            {
                GetText((int)eStatName.Inteli + i).text = DataManager.Instance.playerData.StatsAmounts[i].ToString();
            }
        }

        string GetLineByResult(ActivityData activityData)
        {
            if (activityData.resultType == eResultType.MaxCount)
            {
                Debug.LogError("유효한 결과가 아닙니다.");
                return null;
            }
            if (activityData.activityType == eActivityType.MaxCount)
            {
                Debug.LogError("유효한 활동이 아닙니다.");
                return null;
            }
            Debug.Log($"선택한 활동 종류 {activityData.activityType}");

            if (activityData.resultType == eResultType.BigSuccess)
            {
                return bigSuccessLines[(int)activityData.activityType];
            }
            else if (activityData.resultType == eResultType.Success)
            {
                return successLines[(int)activityData.activityType];
            }
            else
            {
                return failLines[(int)activityData.activityType];
            }
        }

        string GetFaceByResult(eResultType resultType)
        {
            if (resultType == eResultType.MaxCount)
            {
                Debug.LogError("유효한 결과가 아닙니다.");
                return null;
            }

            if (resultType == eResultType.BigSuccess)
                return "glad";
            else if (resultType == eResultType.Success)
                return "basic";
            else
                return "sad";

        }

        /// <summary>
        /// 활동 결과 1 화면 - 캐릭터 대사
        /// </summary>
        /// <returns></returns>
        IEnumerator ShowResult1()
        {
            string str = null;
            string face = null;

            str = GetLineByResult(GameManager.Instance.activityData);
            face = GetFaceByResult(GameManager.Instance.activityData.resultType);

            string path = Util.GetSeasonIllustPath(face);
            charFace.sprite = DataManager.Instance.GetOrLoadSprite(path);

            StartCoroutine(Util.LoadTextOneByOne(str, charLine));
            yield return null;
        }

        /// <summary>
        /// 활동 결과 2 화면 - 나레이션
        /// </summary>
        /// <returns></returns>
        IEnumerator ShowResult2()
        {
            GetGameObject((int)GameObjects.Activity2).SetActive(true);
            GetGameObject((int)GameObjects.Activity1).SetActive(false);

            StringBuilder sb = new StringBuilder();

            if (GameManager.Instance.activityData.activityType == eActivityType.Rest)
            {
                GetGameObject((int)GameObjects.Stats).SetActive(false);
                sb.AppendLine($"{GetResultTypeKor(GameManager.Instance.activityData.resultType)}");
                sb.AppendLine($"스트레스가 + {-GameManager.Instance.activityData.stressValue} 감소했다!");
            }
            else
            {
                GetGameObject((int)GameObjects.Stats).SetActive(true);
                UpdateStatUIs();

                sb.AppendLine($"{GetResultTypeKor(GameManager.Instance.activityData.resultType)}");
                sb.AppendLine($"{GetStatNameKor(GameManager.Instance.activityData.statNames[0])}이 {GameManager.Instance.activityData.statValues[0]} 상승했다.");
                sb.AppendLine($"{GetStatNameKor(GameManager.Instance.activityData.statNames[1])}이 {GameManager.Instance.activityData.statValues[1]} 상승했다.");
            }

            StartCoroutine(Util.LoadTextOneByOne(sb.ToString(), line));

            yield return null;
        }
    }
}