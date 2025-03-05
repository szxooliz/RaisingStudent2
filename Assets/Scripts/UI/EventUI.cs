using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
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
            IMG_CharFace, IMG_NameTag
        }
        enum GameObjects
        {
            Selection, Stats
        }
        #endregion

        private long startingID;
        private long nowEventScriptID;
        private Coroutine coroutine = null;
        private EventScript pastEventScript = null;

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
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
                GetText((int)Texts.TMP_CharLine).ForceMeshUpdate();
            }
            else
            {
                SoundManager.Instance.Play(eSound.SFX_DialogClick);
                coroutine = StartCoroutine(LoadNextDialogue());
            }
        }

        /// <summary>
        /// 대기하고 있는 이벤트 확인 후 실행
        /// </summary>
        void CheckAndShowEvent()
        {
            // 실행할 이벤트 없으면 메인으로 돌아감
            if (EventManager.Instance.EventQueue.Count <= 0)
            {
                StartCoroutine(DelayedTransition());
            }
            else
            {
                InitNewEvent();
                coroutine = StartCoroutine(LoadNextDialogue());
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
            EventManager.Instance.nowEventData = EventManager.Instance.EventQueue.Dequeue();
            startingID = EventManager.Instance.nowEventData.eventScripts.Keys.Min();
            nowEventScriptID = startingID; 

            // 이벤트 타이틀 띄우기
            EventManager.Instance.OnEventStart?.Invoke();

            GetGameObject((int)GameObjects.Selection).SetActive(false);
            GetGameObject((int)GameObjects.Stats).SetActive(false);
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
                if (EventManager.Instance.EventResults.ContainsKey(pastEventScript.index))
                    GetResult();
                else 
                    BranchByType(pastEventScript);
            }

            yield return null;
        }

        void GetResult()
        {
            Debug.Log("이벤트 결과 발생!!");
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
            switch (_eventScript.BranchType)
            {
                case eBranchType.Choice:
                    GetGameObject((int)GameObjects.Selection).SetActive(true);
                    ShowSelection(_eventScript);
                    break;
                case eBranchType.Condition:
                    GetStatCondition(_eventScript);
                    break;
                default:
                    EventScript eventScript = GetNextScript(nowEventScriptID);
                    DisplayScript(eventScript);
                    pastEventScript = eventScript;
                    break;
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
            GetGameObject((int)GameObjects.Selection).SetActive(false);

            // 캐릭터 이미지 사용 여부에 따라 투명도, 파일 설정
            if (_eventScript.NameTag)
            {
                string path = Util.GetSeasonIllustPath(_eventScript);

                GetImage((int)Images.IMG_CharFace).color = new Color(1, 1, 1, 1);
                GetImage((int)Images.IMG_CharFace).sprite = DataManager.Instance.GetOrLoadSprite(path);
                GetImage((int)Images.IMG_NameTag).gameObject.SetActive(true);
            }
            else
            {
                // 나타낼 이미지 없을 때 스프라이트 알파값 0
                GetImage((int)Images.IMG_CharFace).color = new Color(1, 1, 1, 0);
                GetImage((int)Images.IMG_CharFace).sprite = null;
                GetImage((int)Images.IMG_NameTag).gameObject.SetActive(false);
            }

            GetText((int)Texts.TMP_CharName).text = _eventScript.NameTag ?
                DataManager.Instance.GetCharNameKor(_eventScript.Character) : "";
            GetText((int)Texts.TMP_CharLine).text = _eventScript.Line;
        }

        void DeleteOtherScripts(long startIndex, long? endIndex = null)
        {
            while (EventManager.Instance.nowEventData.eventScripts.ContainsKey(startIndex))
            {
                if (endIndex.HasValue && startIndex >= endIndex.Value) break;

                Debug.Log($"{startIndex}번 스크립트는 지웁니다");
                EventManager.Instance.nowEventData.eventScripts.Remove(startIndex++);
            }
        }

        #region 선택지에 따른 분기
        void ShowSelection(EventScript _eventScript)
        {
            SelectScript selectScript = DataManager.Instance.GetData<SelectScript>(_eventScript.BranchIndex);

            GetText((int)Texts.TMP_Select1).text = selectScript.Selection1;
            GetText((int)Texts.TMP_Select2).text = selectScript.Selection2;

            // 기존 리스너 제거
            GetButton((int)Buttons.BTN_Select1).onClick.RemoveAllListeners(); 
            GetButton((int)Buttons.BTN_Select2).onClick.RemoveAllListeners();

            // 리스너 추가
            GetButton((int)Buttons.BTN_Select1).onClick.AddListener(() => OnClickSelection(selectScript.MoveLine1, selectScript.MoveLine2, true));
            GetButton((int)Buttons.BTN_Select2).onClick.AddListener(() => OnClickSelection(selectScript.MoveLine1, selectScript.MoveLine2, false));
        }

        /// <summary>
        /// 선택지 버튼 클릭 후 실행될 내용
        /// </summary>
        /// <param name="nextIndex1">옮길 스크립트 인덱스</param>
        /// <param name="isFirst">첫번째 선택지인가?</param>
        void OnClickSelection(long nextIndex1, long nextIndex2, bool isFirst)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);

            nowEventScriptID = isFirst ? nextIndex1 : nextIndex2;

            // 결과 아닌 스크립트 지우기
            if (isFirst)
                DeleteOtherScripts(nextIndex2);
            else
                DeleteOtherScripts(nextIndex1, nextIndex2);

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
        void GetStatCondition(EventScript _eventScript)
        {
            StatCondition statCondition = DataManager.Instance.GetData<StatCondition>(_eventScript.BranchIndex);

            // 조건 만족하는지 확인한 후 결과에 따라 스크립트 조정
            if (MeasureUpCondition(statCondition))
            {
                nowEventScriptID = statCondition.TrueIndex;
                DeleteOtherScripts(statCondition.FalseIndex);
            }
            else
            {
                nowEventScriptID = statCondition.FalseIndex;
                DeleteOtherScripts(statCondition.TrueIndex, statCondition.FalseIndex);
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

            Debug.Log($"스탯 기준치 비교 결과 : {result}");
            return result;
        }
        #endregion

        #region 이벤트 결과
        void ShowStatResult(EventScript _eventScript)
        {
            if (!EventManager.Instance.EventResults.ContainsKey(_eventScript.index)) return;

            EventResult eventResult = EventManager.Instance.EventResults.GetValueOrDefault(_eventScript.index);
            EventManager.Instance.nowEventData.hasChange = true;
            List<long> result = new()
            {
                eventResult.Inteli, eventResult.Otaku, eventResult.Strength, eventResult.Charming, eventResult.StressValue
            };

            StringBuilder sb = new();
            for (int i = 0; i < (int)eStatNameAll.MaxCount; i++)
            {
                // UI 표시 string
                if (result[i] != 0)
                {
                    if (i == (int)eStatNameAll.Stress)
                    {
                        sb.AppendLine($"{GetStatNameAllKor((eStatNameAll)i)}가 {result[i]}만큼 {ResultString(result[i])}");
                    }
                    else
                    {
                        sb.AppendLine($"{GetStatNameAllKor((eStatNameAll)i)}이 {result[i]}만큼 {ResultString(result[i])}");
                    }
                }

                // 실제 스탯에 반영
                if (i == (int)eStatNameAll.Stress) DataManager.Instance.playerData.StressAmount += result[i];
                else DataManager.Instance.playerData.StatsAmounts[i] += (int)result[i];
            }
            GetGameObject((int)GameObjects.Stats).SetActive(true);
            GetText((int)Texts.TMP_CharName).text = "";

            UpdateStatUIs();
            coroutine = StartCoroutine(Util.LoadTextOneByOne(sb.ToString(), GetText((int)Texts.TMP_CharLine)));
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
                GetText((int)Texts.TMP_Inteli + i).text = DataManager.Instance.playerData.StatsAmounts[i].ToString();
            }
        }

        #endregion
    }
}