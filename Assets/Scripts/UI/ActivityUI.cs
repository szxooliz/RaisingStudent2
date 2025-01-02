using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.Define;

namespace Client
{
    public class ActivityUI : UI_Base, IPointerClickHandler
    {
        private static List<string> _charLines = new List<string>()
        {
            "오늘 집중이 너무 잘 되는데요!",
            "오예 굿굿 저녁으로 맛잇는거 먹어야지",
            "차라리 쿨쿨따 하는게 더 나았겠다"
        };


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

        private string spritePath = "Sprites/Character/";
        private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        private Image charFace;
        private TMPro.TMP_Text charName;
        private TMPro.TMP_Text charLine;
        private TMPro.TMP_Text line;

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

            if (GameManager.Data.playerData.currentStatus == Status.Activity)
            {
                coroutine = StartCoroutine(ShowResult1());
                charName.text = "컴순";
                UpdateStatUIs();
            }
        }

        private void Start()
        {
            if (GameManager.Data.playerData.currentStatus != Status.Activity) return;

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

            if (coroutine != null)
            {
                Debug.Log("대사 출력 중단");
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
                if (GetGameObject((int)GameObjects.Activity1).activeSelf)
                {
                    GetGameObject((int)GameObjects.Activity2).SetActive(true);
                    GetGameObject((int)GameObjects.Activity1).SetActive(false);

                    coroutine = StartCoroutine(ShowResult2());
                }
                else
                {
                    GameManager.Event.CheckEvent();
                    gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 스탯 UI를 업데이트
        /// </summary>
        void UpdateStatUIs()
        {
            for (int i = 0; i < (int)StatName.MaxCount; i++)
            {
                GetText((int)StatName.Inteli + i).text = GameManager.Data.playerData.statsAmounts[i].ToString();
            }
        }

        IEnumerator ShowResult1()
        {
            Debug.Log("ShowResult1 실행");
            string str = GameManager.Data.activityData.activityType == ActivityType.Rest
                ? "임시 - 자체 휴강 캐릭터 대사"
                : _charLines[(int)GameManager.Data.activityData.resultType];

            StartCoroutine(Util.LoadTextOneByOne(str, charLine));

            yield return null;
            Debug.Log("실행 종료, 코루틴 비우기");
        }


        IEnumerator ShowResult2()
        {
            GetGameObject((int)GameObjects.Activity2).SetActive(true);
            GetGameObject((int)GameObjects.Activity1).SetActive(false);

            Debug.Log("ShowResult2 실행");
            string str;

            if (GameManager.Data.activityData.activityType == ActivityType.Rest)
            {
                GetGameObject((int)GameObjects.Stats).SetActive(false);
                str = "스트레스가 " + -GameManager.Data.activityData.stressValue + " 감소했다!";
            }
            else
            {
                str = GetResultTypeKor(GameManager.Data.activityData.resultType) + Environment.NewLine
                      + GetStatNameKor(GameManager.Data.activityData.statName1) + "이 " + GameManager.Data.activityData.stat1Value + " 상승했다." + Environment.NewLine
                      + GetStatNameKor(GameManager.Data.activityData.statName2) + "이 " + GameManager.Data.activityData.stat2Value + " 상승했다." + Environment.NewLine;
            }

            StartCoroutine(Util.LoadTextOneByOne(str, line));

            yield return null;
        }
    }
}