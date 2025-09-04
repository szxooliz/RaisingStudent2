using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;


namespace Client
{
    public class EventUI : UI_Base, IPointerClickHandler
    {
        #region enum
        enum Buttons
        {
            BTN_Select1, BTN_Select2
        }
        enum Texts
        {
            TMP_CharLine, TMP_CharName,
            TMP_Select1, TMP_Select2,
            TMP_Inteli, TMP_Otaku, TMP_Strength, TMP_Charming
        }
        enum Images
        {
            IMG_Background, IMG_CharFace, IMG_NameTag
        }
        enum GameObjects
        {
            Selection, Stats
        }
        #endregion

        private float fadeDuration = 0.5f;
        private long startingID;
        private long nowEventScriptID;
        private Coroutine coroutine = null;
        private EventScript pastEventScript = null;

        [SerializeField] CanvasGroup blackCanvasGroup;
        [SerializeField] CanvasGroup otherCanvasGroup;
        [SerializeField] float fadeInOutTime = 0.1f;


        public override void Init()
        {
            Bind<TMPro.TMP_Text>(typeof(Texts));
            Bind<Image>(typeof(Images));
            Bind<GameObject>(typeof(GameObjects));
            Bind<Button>(typeof(Buttons));
        }

        void OnEnable()
        {
            CheckAndShowEvent();
        }

        public void OnPointerClick(PointerEventData evt)
        {
            // 텍스트 입력 중(또는 첫 번째 클릭 후 전체 노출 대기 중)이라면 무시
            if (Util.nowTexting)
                return;

            // nowTexting == false가 된 시점에만 다음 다이얼로그 진행
            SoundManager.Instance.Play(eSound.SFX_DialogClick);
            coroutine = StartCoroutine(LoadNextDialogue());

            //// 다 보이긴 하는데,, 보이고 바로 다음 LoadNextDialogue이 실행되어버림
            //if (Util.nowTexting)
            //{
            //    StopCoroutine(coroutine);
            //    coroutine = null;
            //    GetText((int)Texts.TMP_CharLine).ForceMeshUpdate();
            //}
            //else
            //{
            //    SoundManager.Instance.Play(eSound.SFX_DialogClick);
            //    coroutine = StartCoroutine(LoadNextDialogue());
            //}

            //if (coroutine != null)
            //{
            //    StopCoroutine(coroutine);
            //    coroutine = null;
            //    GetText((int)Texts.TMP_CharLine).ForceMeshUpdate();
            //}
            //else
            //{
            //    SoundManager.Instance.Play(eSound.SFX_DialogClick);
            //    coroutine = StartCoroutine(LoadNextDialogue());
            //}
        }

        /// <summary>
        /// 대기하고 있는 이벤트 확인 후 실행
        /// </summary>
        void CheckAndShowEvent()
        {
            bool isFinished = DataManager.Instance.playerData.WatchedEventIDList.Contains(12);

            Debug.Log($"<color=yellow> CheckAndShowEvent / 엔딩 보여줄 수 있음? : {isFinished}</color>");

            if (isFinished)
            {
                GameManager.Instance.CheckEndingTurn();
                return;
            }

            // 실행할 이벤트 없으면 메인으로 돌아감
            if (EventManager.Instance.EventQueue.Count <= 0)
            {
                StartCoroutine(DelayedTransition());
            }
            else
            {
                InitNewEvent();
                StartCoroutine(StartNewEvent());
            }
        }

        EventScript GetNextScript(long index)
        {
            return EventManager.Instance.nowEventData.eventScripts.GetValueOrDefault(index, null);
        }

        IEnumerator DelayedTransition()
        {
            yield return new WaitForSeconds(0.3f); // 최종 스크립트 표시 시간 확보
            DataManager.Instance.playerData.CurrentStatus = eStatus.Main;
        }

        /// <summary>
        /// 새로운 이벤트 초기화 및 UI 상태 리셋
        /// </summary>
        void InitNewEvent()
        {
            EventData dequeued = EventManager.Instance.EventQueue.Dequeue();
            EventManager.Instance.nowEventData = new EventData(dequeued);
            startingID = EventManager.Instance.nowEventData.eventScripts.Keys.Min();
            nowEventScriptID = startingID;

            EventManager.Instance.OnEventStart?.Invoke();

            GetGameObject((int)GameObjects.Selection).SetActive(false);
            GetGameObject((int)GameObjects.Stats).SetActive(false);

            //LogManager.Instance.GetNewLogGroup(EventManager.Instance.nowEventData.eventTitle.EventName);
            LogManager.Instance.GetNewClusterGroup(EventManager.Instance.nowEventData.eventTitle.EventName);
        }

        IEnumerator StartNewEvent()
        {
            yield return StartCoroutine(FadeBlackImage());

            coroutine = StartCoroutine(LoadNextDialogue());
        }

        IEnumerator LoadNextDialogue()
        {
            if (pastEventScript == null)
            {
                EventScript eventScript = GetNextScript(nowEventScriptID);
                DisplayScript(eventScript);
                pastEventScript = eventScript;
            }
            else
            {
                if (DataManager.Instance.EventResultDict.ContainsKey(pastEventScript.index))
                    GetResult();
                else 
                    BranchByType(pastEventScript);
            }

            yield return null;
        }

        void GetResult()
        {
            ShowStatResult(pastEventScript);
            EventScript eventScript = GetNextScript(nowEventScriptID);
            pastEventScript = eventScript;
            
            return;
        }

        /// <summary>
        /// 분기 타입별 함수 호출
        /// </summary>
        /// <param name="_eventScript">다음 대사 로드 전 타이밍의 현재 대사</param>
        void BranchByType(EventScript _eventScript)
        {
            if (_eventScript.BranchType == eBranchType.Choice)
            {
                GetGameObject((int)GameObjects.Selection).SetActive(true);
                ShowSelection(_eventScript.BranchIndex);
            }
            else if (_eventScript.BranchType == eBranchType.Condition)
            {
                GetStatCondition(_eventScript.BranchIndex);
            }
            else
            {
                EventScript eventScript = GetNextScript(nowEventScriptID);
                DisplayScript(eventScript);
                pastEventScript = eventScript;
            }
        }

        void DisplayScript(EventScript _eventScript)
        {
            if (_eventScript == null)
            {
                EventManager.Instance.AddWatchedEvent(EventManager.Instance.nowEventData);
                CheckAndShowEvent();
                return;
            }

            UpdateScriptUI(_eventScript);
            StartCoroutine(Util.LoadTextOneByOne(_eventScript.Line, GetText((int)Texts.TMP_CharLine)));
            nowEventScriptID++;
        }

        void UpdateScriptUI(EventScript _eventScript)
        {
            eLineType eLineType = eLineType.SPEAK;
            string path_bg = $"Sprites/UI/BackGround/{_eventScript.Background}";

            string path = Util.GetSeasonIllustPath(_eventScript);

            GetGameObject((int)GameObjects.Selection).SetActive(false);

            // 캐릭터 이미지 사용 여부에 따라 투명도, 파일 설정
            if (path != null)
            {
                GetImage((int)Images.IMG_CharFace).color = new Color(1, 1, 1, 1);
                GetImage((int)Images.IMG_CharFace).sprite = DataManager.Instance.GetOrLoadSprite(path);
            }
            else
            {
                eLineType = eLineType.NARRATION;
                // 나타낼 이미지 없을 때 스프라이트 알파값 0
                GetImage((int)Images.IMG_CharFace).color = new Color(1, 1, 1, 0);
                GetImage((int)Images.IMG_CharFace).sprite = null;
            }

            GetImage((int)Images.IMG_Background).sprite = DataManager.Instance.GetOrLoadSprite(path_bg);
            GetImage((int)Images.IMG_NameTag).gameObject.SetActive(_eventScript.NameTag);
            GetText((int)Texts.TMP_CharName).text = _eventScript.NameTag ?
                Util.GetCharNameKor(_eventScript.Character) : "";
            GetText((int)Texts.TMP_CharLine).text = _eventScript.Line;


            // 로그를 위한 unitLog 객체 생성
            //UnitLog unitLog = new UnitLog(eLineType, _eventScript);
            //LogManager.Instance.GetLastLogGroup().AddUnitLogList(unitLog);
            LogManager.Instance.GetLastClusterGroup().AddLine(eLineType, _eventScript);
        }

        #region 선택지에 따른 분기
        void ShowSelection(long BranchIndex)
        {
            SelectScript selectScript = DataManager.Instance.GetData<SelectScript>(BranchIndex);

            GetText((int)Texts.TMP_Select1).text = selectScript.Selection1;
            GetText((int)Texts.TMP_Select2).text = selectScript.Selection2;

            // 기존 리스너 제거
            GetButton((int)Buttons.BTN_Select1).onClick.RemoveAllListeners(); 
            GetButton((int)Buttons.BTN_Select2).onClick.RemoveAllListeners();

            GetButton((int)Buttons.BTN_Select1).onClick.AddListener(() => OnClickSelection(selectScript, true));
            GetButton((int)Buttons.BTN_Select2).onClick.AddListener(() => OnClickSelection(selectScript, false));
        }

        /// <summary>
        /// 선택지 버튼 클릭 후 실행될 내용
        /// </summary>
        /// <param name="nextIndex1">옮길 스크립트 인덱스</param>
        /// <param name="isFirst">첫번째 선택지인가?</param>
        void OnClickSelection(SelectScript selectScript, bool isFirst)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);

            nowEventScriptID = isFirst ? selectScript.MoveLine1 : selectScript.MoveLine2;
            string selection = isFirst ? selectScript.Selection1 : selectScript.Selection2;

            //UnitLog unitLog = new UnitLog(eLineType.NARRATION, selection);
            //LogManager.Instance.GetLastLogGroup().AddUnitLogList(unitLog);
            LogManager.Instance.GetLastClusterGroup().AddLine(eLineType.NARRATION, selection);


            if (isFirst) // 첫 번째 버튼이면
            {
                EventManager.Instance.DeleteOtherScripts(selectScript.MoveLine2);
                EventManager.Instance.ApplyEvents(true);
            }
            else // 두 번째 버튼이면
            {
                EventManager.Instance.DeleteOtherScripts(selectScript.MoveLine1, selectScript.MoveLine2);
                EventManager.Instance.ApplyEvents(false);
            }

            // 선택지 결과 스크립트 띄우기
            EventScript _eventScript = GetNextScript(nowEventScriptID);
            DisplayScript(_eventScript);
            pastEventScript = _eventScript;
            
            GetGameObject((int)GameObjects.Selection).SetActive(false);
        }
        #endregion

        #region 스탯 조건에 따른 분기
        /// <summary>
        /// 분기 기준치 결과 스크립트 가져오기
        /// </summary>
        void GetStatCondition(long BranchIndex)
        {
            StatCondition statCondition = DataManager.Instance.GetData<StatCondition>(BranchIndex);

            // 조건 만족하는지 확인한 후 결과에 따라 스크립트 조정
            if (MeasureUpCondition(statCondition))
            {
                nowEventScriptID = statCondition.TrueIndex;
                EventManager.Instance.DeleteOtherScripts(statCondition.FalseIndex);
            }
            else
            {
                nowEventScriptID = statCondition.FalseIndex;
                EventManager.Instance.DeleteOtherScripts(statCondition.TrueIndex, statCondition.FalseIndex);
            }

            EventScript eventScript = GetNextScript(nowEventScriptID);
            DisplayScript(eventScript);
            pastEventScript = eventScript;
        }

        /// <summary>
        /// 기준치와 현재 스탯 비교해서 조건 만족하는지 확인
        /// </summary>
        /// <param name="_statCondition"></param>
        /// <returns></returns>
        bool MeasureUpCondition(StatCondition _statCondition)
        {
            bool result = true;
            List<long> conditions = new()
            {
                _statCondition.Inteli, _statCondition.Otaku, _statCondition.Strength, _statCondition.Charming
            };

            for (int i = 0; i < conditions.Count; i++)
            {
                if (DataManager.Instance.playerData.StatsAmounts[i] < (int)conditions[i])
                {
                    Debug.Log($"{(eStatName)i} 스탯 : {DataManager.Instance.playerData.StatsAmounts[i]}, 스탯 기준치 : {conditions[i]}");
                    result = false;
                    break;
                }
            }

            string str = null;
            if (_statCondition.RecordType == eRecordType.Test)
            {
                str = GetTestGrade(conditions[(int)eStatName.Inteli]);
            }
            else if (_statCondition.RecordType == eRecordType.ETC)
            {
                if (result)
                    str = _statCondition.TrueRecord;
                else
                    str = _statCondition.FalseRecord;
            }

            long eventID = EventManager.Instance.nowEventData.eventTitle.index;
            EventManager.Instance.RecordEventResult(eventID, str);

            return result;
        }

        string GetTestGrade(long condition)
        {
            long diff = DataManager.Instance.playerData.StatsAmounts[(int)eStatName.Inteli] - condition;

            if (diff >= 15)
                return "A+";
            else if (diff >= 10 && diff < 15)
                return "A";
            else if (diff >= 0 && diff < 10)
                return "B+";
            else if (diff >= -15 && diff < 0)
                return "B";
            else
                return "C+";
        }
        #endregion

        #region 이벤트 결과
        void ShowStatResult(EventScript _eventScript)
        {
            if (!DataManager.Instance.EventResultDict.ContainsKey(_eventScript.index)) return;

            EventResult eventResult = DataManager.Instance.EventResultDict.GetValueOrDefault(_eventScript.index);
            List<long> result = new()
            {
                eventResult.Inteli, eventResult.Otaku, eventResult.Strength, eventResult.Charming, eventResult.StressValue
            };
            StoreResultStat(result);

            StringBuilder sb = new();
            for (int i = 0; i < (int)eStatNameAll.MaxCount; i++)
            {
                // 음수는 감소했다! 고 이미 표현되므로 마이너스 떼고 표시
                long resultTxt = 0;
                // UI 표시 string
                if (result[i] != 0)
                {
                    if (result[i] < 0)
                        resultTxt = -result[i];
                    else
                        resultTxt = result[i];

                    if (i == (int)eStatNameAll.Stress)
                        sb.AppendLine($"{GetStatNameAllKor((eStatNameAll)i)}가 {resultTxt}만큼 {ResultString(result[i])}");
                    else
                        sb.AppendLine($"{GetStatNameAllKor((eStatNameAll)i)}이 {resultTxt}만큼 {ResultString(result[i])}");
                }

                // 실제 스탯에 반영
                if (i == (int)eStatNameAll.Stress) DataManager.Instance.playerData.StressAmount += result[i];
                else DataManager.Instance.playerData.StatsAmounts[i] += (int)result[i];

            }
            GetGameObject((int)GameObjects.Stats).SetActive(true);
            GetText((int)Texts.TMP_CharName).text = "";

            UpdateStatUIs();
            coroutine = StartCoroutine(Util.LoadTextOneByOne(sb.ToString(), GetText((int)Texts.TMP_CharLine)));
            //UnitLog unitLog = new UnitLog(eLineType.RESULT, sb.ToString());
            //LogManager.Instance.GetLastLogGroup().AddUnitLogList(unitLog);
            LogManager.Instance.GetLastClusterGroup().AddLine(eLineType.RESULT, sb.ToString());
            DataManager.Instance.SaveAllData();

        }

        string ResultString(long stat)
        {
            if (stat < 0) return "감소했다!";
            else          return "증가했다!";
        }

        /// <summary>
        /// 스탯 UI를 업데이트 - 이벤트 결과값 적용
        /// </summary>
        void UpdateStatUIs()
        {
            for (int i = 0; i < (int)eStatName.MaxCount; i++)
            {
                GetText((int)eStatName.Inteli + i).text = (DataManager.Instance.playerData.StatsAmounts[i] + GameManager.Instance.tempResultStat[i]).ToString();
            }
        }
        
        /// <summary> 저장 전 스탯 증가량 저장 </summary>
        void StoreResultStat(List<long> result)
        {
            for (int i = 0; i < (int)eStatName.MaxCount; i++)
            {
                Debug.Log($"이벤트 결과 저장 {(eStatName)i} {result[i]}");

                GameManager.Instance.tempResultStat[i] += result[i];
            }
        }
        #endregion

        #region 이벤트 연출
        private IEnumerator fadeInOut(CanvasGroup target, float durationTime, bool inOut)
        {
            float start, end;
            if (inOut)
            {
                start = 0f;
                end = 1f;
            }
            else
            {
                start = 1f;
                end = 0f;
            }

            float elapsedTime = 0.0f;

            while (elapsedTime < durationTime)
            {
                float alpha = Mathf.Lerp(start, end, elapsedTime / durationTime);

                target.alpha = alpha;

                elapsedTime += Time.deltaTime;

                yield return null;
            }
        }

        public IEnumerator FadeBlackImage()
        {
            otherCanvasGroup.gameObject.SetActive(false);
            blackCanvasGroup.gameObject.SetActive(true);
            yield return fadeInOut(blackCanvasGroup, fadeInOutTime, true);

            float elapsedTime = 0.0f;
            while (elapsedTime < fadeInOutTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return fadeInOut(blackCanvasGroup, fadeInOutTime, false);
            blackCanvasGroup.gameObject.SetActive(false);
            otherCanvasGroup.gameObject.SetActive(true);

        }
        #endregion
    }
}