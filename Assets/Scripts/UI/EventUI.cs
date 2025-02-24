using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
            IMG_CharFace
        }
        enum GameObjects
        {
            Selection
        }
        #endregion

        long startingID;
        long nowEventScriptID;

        Coroutine coroutine = null;

        EventScript pastEventScript = null;

        public override void Init()
        {
            Bind<TMPro.TMP_Text>(typeof(Texts));
            Bind<Image>(typeof(Images));
            Bind<GameObject>(typeof(GameObjects));
        }

        private void OnEnable()
        {
            GetGameObject((int)GameObjects.Selection).SetActive(false);

            CheckAndShowEvent();
        }

        public void OnPointerClick(PointerEventData evt)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
                GetText((int)Texts.TMP_CharLine).ForceMeshUpdate();

                return;
            }
            else
            {
                coroutine = StartCoroutine(LoadNextDialogue());
            }
        }

        /// <summary>
        /// 이벤트 대사 로드
        /// </summary>
        IEnumerator LoadNextDialogue()
        {
            // 다음 대사 로드 전에, 분기 발생하는지 먼저 확인 및 실행
            BranchByType(pastEventScript);

            EventScript eventScript = TryGetNextScript(nowEventScriptID, EventManager.Instance.nowEventData.eventScripts);
            pastEventScript = eventScript;

            yield return null;
        }

        /// <summary>
        /// 다음 대사 가져오기, 있으면 대사 넣고 없으면 null
        /// </summary>
        /// <param name="_index">딕셔너리의 키인 EventScript상의 index</param>
        /// <param name="_eventScripts">해당 이벤트의 대사 목록 딕셔너리</param>
        /// <returns></returns>
        public EventScript TryGetNextScript(long _index, Dictionary<long, EventScript> _eventScripts)
        {
            if (_index - startingID >= _eventScripts.Count)
            {
                Debug.Log($"현재 이벤트 - {EventManager.Instance.nowEventData.title}, 스크립트 총 개수 {_eventScripts.Count}개 끝남");
                return null;
            }
            else
            {
                return _eventScripts.GetValueOrDefault(_index, null);
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
                return;
            }
            else
            {
                RenewEvent();
                coroutine = StartCoroutine(LoadNextDialogue());
            }
        }

        IEnumerator DelayedTransition()
        {
            yield return new WaitForSeconds(0.3f); // 최종 스크립트 표시 시간 확보
            DataManager.Instance.playerData.currentStatus = eStatus.Main;
        }

        /// <summary>
        /// 새로 실행할 이벤트에 맞춰 정보 설정
        /// </summary>
        public void RenewEvent()
        {
            EventManager.Instance.nowEventData = EventManager.Instance.EventQueue.Dequeue();

            // 스크립트 첫 대사 인덱스 초기화
            startingID = EventManager.Instance.nowEventData.eventScripts[0].index;
            nowEventScriptID = startingID; 

            // 이벤트 타이틀 띄우기
            EventManager.Instance.OnEventStart?.Invoke();
        }

        /// <summary>
        /// 분기 타입별 함수 호출
        /// </summary>
        /// <param name="_eventScript">다음 대사 로드 전 타이밍의 현재 대사</param>
        void BranchByType(EventScript _eventScript)
        {
            switch(_eventScript.BranchType)
            {
                case eBranchType.Choice:
                    ShowSelection(_eventScript);
                    break;
                case eBranchType.Condition:
                    ShowStatCondition(_eventScript);
                    break;
                default:
                    return;
            }
        }
        /// <summary>
        /// 선택지 UI 띄우기
        /// </summary>
        void ShowSelection(EventScript _eventScript)
        {
            GetGameObject((int)GameObjects.Selection).SetActive(true);

            long selectID = _eventScript.BranchIndex;
            SelectScript selectScript = DataManager.Instance.GetData<SelectScript>(selectID);

            // TODO : 선택지 텍스트 내용 넣기
            GetText((int)Texts.TMP_Select1).text = null;
            GetText((int)Texts.TMP_Select2).text = null;

            long move1 = 0;
            long move2 = 0;

            GetButton((int)Buttons.BTN_Select1).onClick.AddListener(() => OnClickSelection(move1, move2, true));
            GetButton((int)Buttons.BTN_Select2).onClick.AddListener(() => OnClickSelection(move1, move2, false));
        }

        /// <summary>
        /// 선택지 클릭 후 실행될 내용
        /// </summary>
        /// <param name="move1">옮길 스크립트 인덱스</param>
        /// <param name="isFirst">첫번째 선택지인가?</param>
        void OnClickSelection(long move1, long move2, bool isFirst)
        {
            if (isFirst)
            {
                nowEventScriptID = move1;
                StartCoroutine(DeleteOtherScripts(move1));
            }
            else
            {
                nowEventScriptID = move2;
                StartCoroutine(DeleteOtherScripts(move1, move2));
            }

            GetGameObject((int)GameObjects.Selection).SetActive(false);
            StartCoroutine(LoadNextDialogue());
        }

        /// <summary>
        /// 선택지1 고를 경우 나머지 대사 삭제
        /// </summary>
        /// <param name="move2"></param>
        /// <returns></returns>
        IEnumerator DeleteOtherScripts(long move2)
        {
            // moveline2 이상 키를 가지는 스크립트를 딕셔너리에서 삭제
            long ID = move2;
            while (EventManager.Instance.nowEventData.eventScripts.ContainsKey(ID))
            {
                EventManager.Instance.nowEventData.eventScripts.Remove(ID++);
            }
            yield return null;
        }

        /// <summary>
        /// 선택지2 고를 경우 나머지 대사 삭제
        /// </summary>
        /// <param name="move1"></param>
        /// <param name="move2"></param>
        /// <returns></returns>
        IEnumerator DeleteOtherScripts(long move1, long move2)
        {
            // moveline1 이상 moveline2 미만 키를 가지는 스크립트를 딕셔너리에서 삭제
            long ID = move1;
            while (EventManager.Instance.nowEventData.eventScripts.ContainsKey(ID))
            {
                if (ID >= move2) break;
                EventManager.Instance.nowEventData.eventScripts.Remove(ID++);
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
            if (_eventScript != null)
            {
                DisplayScript(_eventScript);
                StartCoroutine(Util.LoadTextOneByOne(_eventScript.Line, GetText((int)Texts.TMP_CharLine)));
                nowEventScriptID++;
            }
            else
            {
                // 이벤트가 실제 실행 후 끝나는 시점에 봤던 이벤트로 등록
                EventManager.Instance.AddWatchedEvent(EventManager.Instance.nowEventData);
                CheckAndShowEvent();
            }
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
            }
            else
            {
                // 나타낼 이미지 없을 때 스프라이트 알파값 0
                GetImage((int)Images.IMG_CharFace).color = new Color(1, 1, 1, 0);
                GetImage((int)Images.IMG_CharFace).sprite = null;
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