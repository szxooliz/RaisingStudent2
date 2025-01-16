using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : UI_Base
{
    enum Texts
    {
        TMP_CharLine, TMP_CharName
    }
    enum Images
    {
        IMG_CharFace,IMG_Bubble
    }

    private string charSpritePath = "Sprites/Character/";
    //private string bubbleSpritePath = "";
    private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

    public override void Init()
    {
        Bind<TMPro.TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
    }
    /// <summary>
    /// 이벤트 실행
    /// </summary>
    public void ShowEvent()
    {
        // 미완성
        DataManager.Instance.playerData.currentStatus = Define.Status.Event;

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
