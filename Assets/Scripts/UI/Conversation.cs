using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace Client
{
    public class Conversation : UI_Base, IPointerClickHandler
    {
        enum CharImages
        {
            IMG_CharFace, IMG_CharBubble
        }

        enum CharTexts
        {
            TMP_CharLine
        }

        private long maxCount; // 임시 - 아마 선택한 캐릭터 스크립트 개수만큼 카운트 해야 됨..

        private Image charFace;
        private Image charBubble;
        private TMPro.TMP_Text charLine;

        RectTransform bubbleRect;

        Vector3 originalScale;
        Vector3 smallScale;

        private Coroutine coroutine = null;

        public override void Init()
        {
            Bind<UnityEngine.UI.Image>(typeof(CharImages));
            Bind<TMPro.TMP_Text>(typeof(CharTexts));

            maxCount = 2; // 임시
        }

        private void Start()
        {
            charFace = GetImage((int)CharImages.IMG_CharFace);
            charBubble = GetImage((int)CharImages.IMG_CharBubble);
            charLine = GetText((int)CharTexts.TMP_CharLine);

            bubbleRect = charBubble.rectTransform;

            originalScale = bubbleRect.localScale;
            smallScale = originalScale * 0.1f;

            charFace.alphaHitTestMinimumThreshold = 0.1f;

            StartCoroutine(ResetBubble(0));
        }

        /// <summary>
        /// 오브젝트에서 포인터를 누르고 뗄 때 호출됨
        /// </summary>
        /// <param name="evt"></param>
        public void OnPointerClick(PointerEventData evt)
        {
            if (coroutine != null)
            {
                Debug.Log("코루틴 이미 실행 중");
                return;
            }
            else
            {
                coroutine = StartCoroutine(ResetBubble(Random.Range(0, (int)maxCount)));
            }
        }

        /// <summary>
        /// 인터랙션 시 말풍선 새로 고침
        /// </summary>
        IEnumerator ResetBubble(int index)
        {
            index = UnityEngine.Random.Range(0, 33); // 전체 개수 가져오고 싶은데
            Script script = DataManager.Instance.GetData<Script>(index);

            // 임시: 캐릭터 종류 관련 로직 정해지면 수정
            //string charType = script.Character; 

            if (script == null) 
            {
                yield return null;
            }

            try
            {
                string path = Util.GetSeasonIllustPath(script);
                Sprite sprite = DataManager.Instance.GetOrLoadSprite(path);

                StartCoroutine(AnimateBubble(script, sprite));
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to update bubble: {e.Message}");
                coroutine = null;
            }

            yield return null;
        }

        /// <summary>
        /// 말풍선 애니메이션
        /// </summary>
        /// <returns></returns>
        IEnumerator AnimateBubble(Script _script, Sprite _sprite)
        {
            charLine.alpha = 0f;

            Sequence seq = DOTween.Sequence();

            seq
                .Append(bubbleRect.DOScale(smallScale, 0.3f).SetEase(Ease.InOutQuad))
                .AppendCallback(()=>
                {
                    charFace.sprite = _sprite;
                })
                .Append(bubbleRect.DOScale(originalScale, 0.3f).SetEase(Ease.OutBounce))
                .AppendCallback(() =>
                {
                    charLine.alpha = 1f;
                    charLine.text = _script.Line;
                });

            // 코루틴 종료 후 상태 초기화
            yield return seq.WaitForCompletion();
            coroutine = null;
        }
    }
}
