using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventUI : UI_Base, IPointerClickHandler
{
    enum Texts
    {
        TMP_CharLine, TMP_CharName
    }
    enum Images
    {
        IMG_CharFace //, IMG_Bubble 딱히 필요 없을듯
    }

    private string charSpritePath = "Sprites/Character/";
    private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

    int index = 0;
    Coroutine coroutine = null;
    int currentEventID;


    public override void Init()
    {
        Bind<TMPro.TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
    }
    private void Start()
    {
        ShowEvent();
    }
    /// <summary>
    /// 이벤트 실행 : UI 초기화
    /// </summary>
    public void ShowEvent()
    {
        index = 0;
        currentEventID = EventManager.Instance.currentEventID;
        coroutine = StartCoroutine(LoadNextDialogue(index++));
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
            coroutine = StartCoroutine(LoadNextDialogue(index++));
        }
        
    }

    /// <summary>
    /// 다음 이벤트 대사 로드
    /// </summary>
    /// <param name="_index"></param>
    IEnumerator LoadNextDialogue(int _index)
    {
        // 스크립트 레코드 가져오기
        EventScript eventScript = DataManager.Instance.GetData<EventScript>(_index);

        // 이벤트 스크립트 없으면 메인으로 전환
        if (eventScript == null)
        {
            Debug.Log("eventScript is null");

            DataManager.Instance.playerData.currentStatus = Define.Status.Main;
            coroutine = null;

            yield break;
        }

        // 현재 해당하는 이벤트 끝날 시 메인으로 전환
        if (currentEventID != eventScript.EventNum)
        {
            Debug.Log($"current Event ID : {currentEventID}, evntScript : {eventScript.EventNum}");

            DataManager.Instance.playerData.currentStatus = Define.Status.Main;
            coroutine = null;

            yield break;
        }

        GetText((int)Texts.TMP_CharLine).text = eventScript.Line;
        GetText((int)Texts.TMP_CharName).text = eventScript.NameTag ? eventScript.Character : "";
        
        // 캐릭터 이미지 사용 여부에 따라 투명도, 파일 설정
        if (eventScript.NameTag)
        {
            GetImage((int)Images.IMG_CharFace).color = new Color(1, 1, 1, 1);
            GetImage((int)Images.IMG_CharFace).sprite = GetOrLoadSprite($"{charSpritePath}{eventScript.Character}_{eventScript.Face}");
        }
        else
        {
            // 나타낼 이미지 없을 때 스프라이트 알파값 0
            GetImage((int)Images.IMG_CharFace).color = new Color(1, 1, 1, 0);
            GetImage((int)Images.IMG_CharFace).sprite = null;
        }

        StartCoroutine(Util.LoadTextOneByOne(eventScript.Line, GetText((int)Texts.TMP_CharLine)));

        yield return null;
        //coroutine = null;
    }
    
    Sprite GetOrLoadSprite(string _path) 
    {
        if (spriteCache.TryGetValue(_path, out Sprite cachedSprite))
        {
            // 캐싱된 스프라이트 반환
            return cachedSprite;
        }

        Sprite loadedSprite = Resources.Load<Sprite>(_path);
        if (loadedSprite == null)
        {
            throw new System.Exception($"Sprite not found at path: {_path}");
        }

        // 로드된 스프라이트를 캐싱
        spriteCache[_path] = loadedSprite;
        return loadedSprite;
    }
}
