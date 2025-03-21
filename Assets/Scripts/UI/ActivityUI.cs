using System;
using System.Collections;
using System.Collections.Generic;
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

        [SerializeField]
        private List<string> _charLines = new List<string>()
        {
            "오늘 집중이 너무 잘 되는데요!",
            "오예 굿굿 저녁으로 맛잇는거 먹어야지",
            "차라리 쿨쿨따 하는게 더 나았겠다"
        };

        [SerializeField]
        private List<string> _restLines = new()
        {
            "스트레스가 싹 풀렸어요!! 겡끼삥삥~!",
            "역시 최고의 대학은 침대예요!",
            "으우... 시간만 낭비한 것 같아요..."
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

        private Image charFace;          // 캐릭터 이미지
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

        /// <summary>
        /// 활동 결과 1 화면 - 캐릭터 대사
        /// </summary>
        /// <returns></returns>
        IEnumerator ShowResult1()
        {
            string str = null;
            string face = null;

            if (DataManager.Instance.activityData.activityType == eActivityType.Rest)
                str = _restLines[(int)DataManager.Instance.activityData.resultType];
            else
                str = _charLines[(int)DataManager.Instance.activityData.resultType];

            if (DataManager.Instance.activityData.resultType == eResultType.BigSuccess)
                face = "glad";
            else if (DataManager.Instance.activityData.resultType == eResultType.Success)
                face = "basic";
            else
                face = "sad";

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

            string str; // TODO : StringBuilder로 바꾸기

            if (DataManager.Instance.activityData.activityType == eActivityType.Rest)
            {
                GetGameObject((int)GameObjects.Stats).SetActive(false);
                str = GetResultTypeKor(DataManager.Instance.activityData.resultType) + Environment.NewLine
                    + "스트레스가 " + -DataManager.Instance.activityData.stressValue + " 감소했다!";
            }
            else
            {
                GetGameObject((int)GameObjects.Stats).SetActive(true);
                UpdateStatUIs();

                str = GetResultTypeKor(DataManager.Instance.activityData.resultType) + Environment.NewLine
                      + GetStatNameKor(DataManager.Instance.activityData.statNames[0]) + "이 " + DataManager.Instance.activityData.statValues[0] + " 상승했다." + Environment.NewLine
                      + GetStatNameKor(DataManager.Instance.activityData.statNames[1]) + "이 " + DataManager.Instance.activityData.statValues[1] + " 상승했다." + Environment.NewLine;
            }

            StartCoroutine(Util.LoadTextOneByOne(str, line));

            yield return null;
        }
    }
}