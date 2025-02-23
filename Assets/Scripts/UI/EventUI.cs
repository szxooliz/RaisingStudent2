using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class EventUI : UI_Base, IPointerClickHandler
    {
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

        long startingID;
        long nowEventScriptID;

        Coroutine coroutine = null;

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
        /// <param name="_index"></param>
        IEnumerator LoadNextDialogue()
        {
            // 가져온 이벤트 스크립트가 null이면 CheckAndShowEvent 실행해서 새 이벤트 있는지 체크하고 바로 세팅

            EventScript eventScript = TryGetNextScript(nowEventScriptID, EventManager.Instance.nowEventData.eventScripts);
            BranchByType(eventScript);

            if (eventScript != null)
            {
                DisplayScript(eventScript);
                StartCoroutine(Util.LoadTextOneByOne(eventScript.Line, GetText((int)Texts.TMP_CharLine)));
                nowEventScriptID++;
            }
            else
            {
                // 이벤트가 실제 실행 후 끝나는 시점에 봤던 이벤트로 등록
                EventManager.Instance.AddWatchedEvent(EventManager.Instance.nowEventData);
                CheckAndShowEvent();
            }

            yield return null;
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
            yield return new WaitForSeconds(0.5f); // 최종 스크립트 표시 시간 확보
            DataManager.Instance.playerData.currentStatus = eStatus.Main;
        }

        /// <summary>
        /// 새로 실행할 이벤트 정보 설정
        /// </summary>
        public void RenewEvent()
        {
            EventManager.Instance.nowEventData = EventManager.Instance.EventQueue.Dequeue();

            // 스크립트 첫 대사 인덱스 초기화
            nowEventScriptID = EventManager.Instance.nowEventData.eventScripts[0].index; 
            startingID = EventManager.Instance.nowEventData.eventScripts[0].index;

            // 이벤트 타이틀 띄우기
            EventManager.Instance.OnEventStart?.Invoke();
        }


        /// <summary>
        /// 다음 대사 가져오기, 있으면 대사 넣고 없으면 null
        /// </summary>
        /// <param name="_eventScripts"></param>
        public EventScript TryGetNextScript(long _index, List<EventScript> _eventScripts)
        {
            if (_index - startingID >= _eventScripts.Count)
            {
                Debug.Log($"현재 이벤트 - {EventManager.Instance.nowEventData.title}, 스크립트 총 개수 {_eventScripts.Count}개 끝남");
                return null;
            }
            else
            {
                try
                {
                    return _eventScripts[(int)(_index - startingID)];
                }
                catch
                {
                    return null;
                }
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
        /// 분기 타입별 함수 호출
        /// </summary>
        /// <param name="_eventScript"></param>
        void BranchByType(EventScript _eventScript)
        {
            switch(_eventScript.BranchType)
            {
                case eBranchType.Choice:
                    ShowSelection(_eventScript);
                    break;
                case eBranchType.Condition:
                    ShowStatChange(_eventScript);
                    break;
                default:
                    return;
            }
        }
        /// <summary>
        /// 선택지 띄우기
        /// </summary>
        void ShowSelection(EventScript _eventScript)
        {
            GetGameObject((int)GameObjects.Selection).SetActive(true);

            // 스크립트에 딸린 분기 인덱스 참고해서 선택지 테이블의 정보 가져오기

        }

        void ShowStatChange(EventScript _eventScript)
        {
            // 스크립트에 딸린 분기 인덱스 참고해서 스탯기준치 테이블의 정보 가져오기

            UpdateStatUIs();

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