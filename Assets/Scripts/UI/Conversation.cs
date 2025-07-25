using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using static Client.SystemEnum;

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

        private readonly long MAXCOUNT = 52; // TODO : 임시 - 아마 선택한 캐릭터 스크립트 개수만큼 카운트 해야 됨..

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
            StartCoroutine(ResetBubble());

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
                SoundManager.Instance.Play(eSound.SFX_DialogClick);
                coroutine = StartCoroutine(ResetBubble());
            }
        }

        /// <summary>
        /// 인터랙션 시 말풍선 새로 고침
        /// </summary>
        public IEnumerator ResetBubble()
        {
            int index = UnityEngine.Random.Range(0, (int)MAXCOUNT);
            Script script = DataManager.Instance.GetData<Script>(index);

            if (script == null) 
            {
                yield break;
            }

            yield return new WaitForSeconds(0.3f);

            try
            {
                string path = Util.GetSeasonIllustPath(script);
                Sprite sprite = DataManager.Instance.GetOrLoadSprite(path);

                StartCoroutine(AnimateBubble(script, sprite));
            }
            catch (System.Exception e)
            {
                coroutine = null;
                Debug.LogError($"Failed to update bubble: {e.Message}");
            }
            yield break;
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
