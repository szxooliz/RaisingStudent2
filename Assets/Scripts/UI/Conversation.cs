using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Client
{
    public class Conversation : UI_Base, IPointerClickHandler
    {
        /*
        1. 기본 세팅: 말풍선 기본으로 항상 떠 있음
        2. 인터랙션 하면 사라졌다가 재생성
            - 재생성 시 대사 & 캐릭터 표정 불러오기
        */
        enum CharImages
        {
            IMG_CharFace, IMG_CharBubble
        }

        enum CharTexts
        {
            TMP_CharLine
        }

        private Script script;
        private long maxCount;
        string spritePath = "Sprites/";

        private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        public override void Init()
        {
            Bind<UnityEngine.UI.Image>(typeof(CharImages));
            Bind<TMPro.TMP_Text>(typeof(CharTexts));

            maxCount = 2; // temp value
        }

        /// <summary>
        /// 오브젝트에서 포인터를 누르고 뗄 때 호출됨
        /// </summary>
        /// <param name="evt"></param>
        public void OnPointerClick(PointerEventData evt)
        {
            // TODO: 말풍선 위에도 적용되는지 확인하기
            ResetBubble(Random.Range(0, (int)maxCount));
        }

        /// <summary>
        /// 인터랙션 시 말풍선 새로 고침
        /// </summary>
        void ResetBubble(int index)
        {
            index = UnityEngine.Random.Range(0, 2);
            Script script = DataManager.Instance.GetData<Script>(index);

            // 임시: 캐릭터 종류 관련 로직 정해지면 수정
            string charType = "Comsoon"; 

            if (script == null) 
            {
                return;
            }

            try
            {
                string path = spritePath + charType + "_" + script.Face;
                Sprite sprite = GetOrLoadSprite(path);

                GetText((int)CharTexts.TMP_CharLine).text = script.Line;
                GetImage((int)CharImages.IMG_CharFace).sprite = sprite;

                Debug.Log($"Bubble updated: Line = {script.Line}, Face = {script.Face}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to update bubble: {e.Message}");
            }
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
}
