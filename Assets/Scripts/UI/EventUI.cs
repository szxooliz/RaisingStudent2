using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
            TMP_Select1, TMP_Select2
        }
        enum Images
        {
            IMG_CharFace, IMG_NameTag
        }
        enum GameObjects
        {
            Selection
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

        private void OnEnable()
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
        public void CheckAndShowEvent()
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

        /// <summary>
        /// 이벤트 대사 로드
        /// </summary>
        IEnumerator LoadNextDialogue()
        {
            if (pastEventScript != null)
            {
                // 다음 대사 로드 전에, 분기 발생하는지 먼저 확인 및 실행
                Debug.Log($"이전 {pastEventScript.index}번 스크립트의 분기타입 {pastEventScript.BranchType.ToString()}");
                BranchByType(pastEventScript);
            }

            pastEventScript = TryGetNextScript(nowEventScriptID, EventManager.Instance.nowEventData.eventScripts);
            ShowScript(pastEventScript);

            yield return null;
        }

        /// <summary>
        /// 다음 대사 가져오기, 있으면 대사 넣고 없으면 null
        /// </summary>
        /// <param name="_index">딕셔너리의 키인 EventScript상의 index</param>
        /// <param name="_eventScripts">해당 이벤트의 대사 목록 딕셔너리</param>
        /// <returns></returns>
        public EventScript TryGetNextScript(long index, Dictionary<long, EventScript> eventScripts)
        {
            return index - startingID < eventScripts.Count ? eventScripts.GetValueOrDefault(index, null) : null;
        }

        IEnumerator DelayedTransition()
        {
            yield return new WaitForSeconds(0.3f); // 최종 스크립트 표시 시간 확보
            DataManager.Instance.playerData.currentStatus = eStatus.Main;
        }

        /// <summary>
        /// 새로 실행할 이벤트에 맞춰 정보 설정
        /// </summary>
        public void InitNewEvent()
        {
            EventManager.Instance.nowEventData = EventManager.Instance.EventQueue.Dequeue();

            // 스크립트 첫 대사 인덱스 초기화
            startingID = EventManager.Instance.nowEventData.eventScripts.Keys.Min();
            nowEventScriptID = startingID; 

            // 이벤트 타이틀 띄우기
            EventManager.Instance.OnEventStart?.Invoke();

            GetGameObject((int)GameObjects.Selection).SetActive(false);
        }

        /// <summary>
        /// 분기 타입별 함수 호출
        /// </summary>
        /// <param name="eventScript">다음 대사 로드 전 타이밍의 현재 대사</param>
        void BranchByType(EventScript eventScript)
        {
            if (eventScript == null) return;

            Debug.Log($"분기 타입 체크 {eventScript.BranchType.ToString()}");
            switch(eventScript.BranchType)
            {
                case eBranchType.Choice:
                    GetGameObject((int)GameObjects.Selection).SetActive(true);
                    ShowSelection(eventScript);
                    break;
                case eBranchType.Condition:
                    ShowStatCondition(eventScript);
                    break;
            }
        }
        /// <summary>
        /// 선택지 UI 띄우기
        /// </summary>
        void ShowSelection(EventScript _eventScript)
        {
            SelectScript selectScript = DataManager.Instance.GetData<SelectScript>(_eventScript.BranchIndex);

            Debug.Log($"선택지 무브라아ㅏ아인 {selectScript.MoveLine1}, {selectScript.MoveLine2}");

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
        /// 선택지 클릭 후 실행될 내용
        /// </summary>
        /// <param name="nextIndex1">옮길 스크립트 인덱스</param>
        /// <param name="isFirst">첫번째 선택지인가?</param>
        void OnClickSelection(long nextIndex1, long nextIndex2, bool isFirst)
        {
            SoundManager.Instance.Play(eSound.SFX_Positive);

            nowEventScriptID = isFirst ? nextIndex1 : nextIndex2;

            if (isFirst)
                StartCoroutine(DeleteOtherScripts(nextIndex1));
            else
                StartCoroutine(DeleteOtherScripts(nextIndex1, nextIndex2));

            GetGameObject((int)GameObjects.Selection).SetActive(false);
            StartCoroutine(LoadNextDialogue());
        }


        /// <summary>
        /// 선택지 결과 외의 나머지 대사 삭제
        /// </summary>
        /// <param name="move1"></param>
        /// <param name="move2"></param>
        /// <returns></returns>
        IEnumerator DeleteOtherScripts(long startIndex, long? endIndex = null)
        {
            while (EventManager.Instance.nowEventData.eventScripts.ContainsKey(startIndex))
            {
                if (endIndex.HasValue && startIndex >= endIndex.Value) break;
                EventManager.Instance.nowEventData.eventScripts.Remove(startIndex++);
            }
            yield return null;
        }

        /// <summary>
        /// 스탯 기준치에 따라 내용이 바뀔 경우
        /// </summary>
        /// <param name="_eventScript"></param>
        void ShowStatCondition(EventScript _eventScript)
        {
            // 스크립트에 딸린 분기 인덱스 참고해서 스탯기준치 테이블의 정보 가져오기
            UpdateStatUIs();
        }

        /// <summary>
        /// 일반 대사 보여주기
        /// </summary>
        /// <param name="_eventScript"></param>
        void ShowScript(EventScript _eventScript)
        {
            if (_eventScript == null)
            {
                EventManager.Instance.AddWatchedEvent(EventManager.Instance.nowEventData);
                CheckAndShowEvent();
                return;
            }

            DisplayScript(_eventScript);
            StartCoroutine(Util.LoadTextOneByOne(_eventScript.Line, GetText((int)Texts.TMP_CharLine)));
            nowEventScriptID++;
        }

        /// <summary>
        /// 스크립트 내용 맞춰서 UI 띄우기
        /// </summary>
        /// <param name="_eventScript"></param>
        void DisplayScript(EventScript _eventScript)
        {
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

        /// <summary>
        /// 스탯 UI를 업데이트 - 이벤트 결과값 적용
        /// </summary>
        void UpdateStatUIs()
        {
            for (int i = 0; i < (int)eStatName.MaxCount; i++)
            {
                GetText((int)eStatName.Inteli + i).text = DataManager.Instance.playerData.statsAmounts[i].ToString();
            }
        }
    }
}