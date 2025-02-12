using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.Define;

public class EventUI : UI_Base, IPointerClickHandler
{
    enum Texts
    {
        TMP_CharLine, TMP_CharName
    }
    enum Images
    {
        IMG_CharFace
    }

    EventData nowEventData;
    long nowEventScriptID;

    Coroutine coroutine = null;

    public override void Init()
    {
        Bind<TMPro.TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
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

            return;
        }
        else
        {
            coroutine = StartCoroutine(LoadNextDialogue());
        }
    }

    /// <summary>
    /// 새로 실행할 이벤트 정보 설정
    /// </summary>
    public void RenewEvent()
    {
        // 보여줄 이벤트 데이터 설정
        // 첫 대사 인덱스를 위해 위 이벤트 데이터의 이벤트 스크립트 맨 앞 인덱스로 설정

        nowEventData = EventManager.Instance.eventQueue.Dequeue();
        nowEventScriptID = nowEventData.eventScripts[0].index; // 스크립트 인덱스 초기화

        Debug.Log($"실행할 이벤트 : {nowEventData.title}, 앞으로 남은 이벤트 : {EventManager.Instance.eventQueue.Count}개");
    }

    /// <summary>
    /// 대기하고 있는 이벤트 확인 후 실행
    /// </summary>
    public void CheckAndShowEvent()
    {
        // 실행할 이벤트 없으면 메인으로 돌아감
        if (EventManager.Instance.eventQueue.Count <= 0)
        {
            Debug.Log("대기 중인 이벤트 없음, 메인으로 돌아갑니다");

            // 임시
            // 이벤트, 활동, 전부 상관없이 턴 안에 하는 모든 프로세스 종료 후 메인으로 돌아오면 턴 증가
            //GameManager.Instance.NextTurn();

            DataManager.Instance.playerData.currentStatus = eStatus.Main;
            return;
        }

        RenewEvent();
        coroutine = StartCoroutine(LoadNextDialogue());
    }

    /// <summary>
    /// 다음 대사 가져오기, 있으면 대사 넣고 없으면 null << 이게 맞나?
    /// </summary>
    /// <param name="eventScripts"></param>
    public EventScript TryGetNextScript(long index, List<EventScript> eventScripts)
    {
        EventScript eventScript = null;

        if (eventScripts[(int)index] != null)
        {
            eventScript = eventScripts[(int)index];
        }

        return eventScript;
    }

    /// <summary>
    /// 이벤트 대사 로드
    /// </summary>
    /// <param name="_index"></param>
    IEnumerator LoadNextDialogue()
    {
        // 클릭할 때 다음 스크립트 띄우기 -> 스크립트 인덱스 증가를 어디서 해야할지.. 여기 안에서 하는게 안전할 듯
        // 가져온 이벤트스크립트가 null이면 CheckAndShowEvent 실행해서 새 이벤트 있는지 체크하고 바로 세팅

        EventScript eventScript = TryGetNextScript(nowEventScriptID, nowEventData.eventScripts);

        if (eventScript != null) 
        {
            DisplayScript(eventScript);
            nowEventScriptID++;
        }
        else
        {
            CheckAndShowEvent();
        }

        StartCoroutine(Util.LoadTextOneByOne(eventScript.Line, GetText((int)Texts.TMP_CharLine)));
        yield return null;
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

        GetText((int)Texts.TMP_CharName).text = _eventScript.NameTag ? _eventScript.Character : "";
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
