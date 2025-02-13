using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class ActivityUI : UI_Base, IPointerClickHandler
    {
        [SerializeField]
        private List<string> _charLines = new List<string>()
        {
            "오늘 집중이 너무 잘 되는데요!",
            "오예 굿굿 저녁으로 맛잇는거 먹어야지",
            "차라리 쿨쿨따 하는게 더 나았겠다"
        };
        [SerializeField] private string _restLine = "F 안 맞을 정도로만 쉬어도 돼~ ㅋㅋ";


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

        private Image charFace;          // 캐릭터 이미지
        private TMPro.TMP_Text charName; // 캐릭터 이름표
        private TMPro.TMP_Text charLine; // 캐릭터 대사
        private TMPro.TMP_Text line;     // 결과 나레이션

        private Coroutine coroutine = null;

        // 기획 조정 용
        [SerializeField] float charDuration = 5f;
        [SerializeField] float narDuration = 5f;

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

            if (DataManager.Instance.playerData.currentStatus == eStatus.Activity)
            {
                coroutine = StartCoroutine(ShowResult1());
                charName.text = "컴순";
                UpdateStatUIs();
            }
        }

        private void Start()
        {
            if (DataManager.Instance.playerData.currentStatus != eStatus.Activity) return;

            GetGameObject((int)GameObjects.Activity1).SetActive(true);
            GetGameObject((int)GameObjects.Activity2).SetActive(false);
        }

        /// <summary>
        /// 오브젝트에서 포인터를 누르고 뗄 때 호출됨
        /// </summary>
        /// <param name="evt"></param>
        public void OnPointerClick(PointerEventData evt)
        {
            /*
            1. ShowResult1() 실행 중 눌렀을 때 중단하고 텍스트 전체 보임
            2. ShowResult1() 종료 후 눌렀을 때 Activity2 활성화 후 ShowResult2() 실행
            3. ShowResult2() 실행 중 눌렀을 때 중단하고 텍스트 전체 보임
            4. ShowResult2() 종료 후 눌렀을 때 ActivityUI 비활성화
             */

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
                // 다음 단계로 넘어가기
                if (GetGameObject((int)GameObjects.Activity1).activeSelf)
                {
                    GetGameObject((int)GameObjects.Activity2).SetActive(true);
                    GetGameObject((int)GameObjects.Activity1).SetActive(false);

                    coroutine = StartCoroutine(ShowResult2());
                }
                else // 메인으로 돌아가기
                {
                    EventManager.Instance.CheckAbleEvent();
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
                GetText((int)eStatName.Inteli + i).text = DataManager.Instance.playerData.statsAmounts[i].ToString();
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
            {
                str = _restLine;
            }
            else
            {
                str = _charLines[(int)DataManager.Instance.activityData.resultType];

                switch (DataManager.Instance.activityData.resultType)
                {
                    case eResultType.BigSuccess:
                        face = "glad";
                        break;
                    case eResultType.Success:
                        face = "basic";
                        break;
                    case eResultType.Failure:
                        face = "sad";
                        break;
                }
                string path = Util.GetSeasonIllustPath(face);
                charFace.sprite = DataManager.Instance.GetOrLoadSprite(path);
            }

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

            string str;

            if (DataManager.Instance.activityData.activityType == eActivityType.Rest)
            {
                GetGameObject((int)GameObjects.Stats).SetActive(false);
                str = "스트레스가 " + -DataManager.Instance.activityData.stressValue + " 감소했다!";
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