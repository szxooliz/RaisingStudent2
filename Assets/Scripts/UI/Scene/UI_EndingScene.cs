using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.Define;

namespace Client
{
    public class UI_EndingScene : UI_Scene, IPointerClickHandler
    {
        enum Buttons
        {
            BTN_Menu, BTN_Log,
        }

        enum Images
        {
            IMG_Illustration,
            IMG_CharFace,
            IMG_Bubble,
            IMG_Name,
        }

        enum Texts
        {
            TMP_CharName, TMP_CharLine,
        }

        private string spritePath = "Sprites/Character/";
        private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        int index = 0; // 엔딩 대사 index

        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<Image>(typeof(Images));
            Bind<TMPro.TMP_Text>(typeof(Texts));

            BindButton();
            CheckEnding();

            LoadNextScript();
        }

        void BindButton()
        {
            BindEvent(GetButton((int)Buttons.BTN_Menu).gameObject, OnClickMenuBtn);
            BindEvent(GetButton((int)Buttons.BTN_Log).gameObject, OnClickLogBtn);
        }

        #region 버튼 이벤트
        void OnClickMenuBtn(PointerEventData evt)
        {
            Debug.Log("메뉴 버튼 클릭");
            UI_Manager.Instance.ShowPopupUI<UI_MenuPopup>();
        }

        void OnClickLogBtn(PointerEventData evt)
        {
            Debug.Log("로그 버튼 클릭");
            UI_Manager.Instance.ShowPopupUI<UI_LogPopup>();
        }
        #endregion

        /// <summary>
        /// 오브젝트에서 포인터를 누르고 뗄 때 호출됨
        /// </summary>
        /// <param name="evt"></param>
        public void OnPointerClick(PointerEventData evt)
        {
            index += 1;
            LoadNextScript();
        }

        /// <summary>
        /// 엔딩 계산 함수
        /// </summary>
        void CheckEnding()
        {
            int highStatsCount = 0;
            bool[] highStats = { false, false, false, false };

            for (int i = 0; i < 4; i++)
            {
                if ((int)DataManager.Instance.playerData.statsAmounts[i] >= 85)
                {
                    highStats[i] = true;
                    highStatsCount++;
                }
            }

            if (highStatsCount == 4)
            {
                Debug.Log("대학원 엔딩");
            }
            else if (highStatsCount >= 2)
            {
                if (highStats[0] && highStats[3])
                {
                    Debug.Log("대기업 SI 취업 엔딩");
                }
                else if (highStats[0] && highStats[1])
                {
                    Debug.Log("게임회사 취업 엔딩");
                }
                else if (highStats[3] && highStats[1])
                {
                    Debug.Log("버튜버 엔딩");
                }
                else if (highStats[2] && highStats[1])
                {
                    Debug.Log("프로게이머 엔딩");
                }
            }
            else
            {
                Debug.Log("홈프로텍터 엔딩");
            }
        }

        /// <summary>
        /// 다음 스크립트 로드
        /// </summary>
        void LoadNextScript()
        {
            EndingScript endingScript = DataManager.Instance.GetData<EndingScript>(index);
            if (endingScript == null)
            {
                return;
            }

            LoadNextDialogue(endingScript);
            LoadIllustration(endingScript);
        }

        /// <summary>
        /// 다음 엔딩 대사 로드
        /// </summary>
        /// <param name="endingScript"></param>
        void LoadNextDialogue(EndingScript endingScript)
        {
            GetText((int)Texts.TMP_CharLine).text = endingScript.Line;
            
            if (endingScript.NameTag)
            {
                GetText((int)Texts.TMP_CharName).text = endingScript.Character;
                GetImage((int)Images.IMG_Name).gameObject.SetActive(true);
            }
            else
            {
                GetText((int)Texts.TMP_CharName).text = "";
                GetImage((int)Images.IMG_Name).gameObject.SetActive(false);
            }

            string path;
            if (endingScript.Face == "none")
            { // 임시
                path = spritePath + "Comsoon_basic";
            }
            else
            {
                path = spritePath + "Comsoon_" + endingScript.Face;
            }
            GetImage((int)Images.IMG_CharFace).sprite = GetOrLoadSprite(path);
        }

        /// <summary>
        /// 다음 일러스트 로드
        /// </summary>
        /// <param name="endingScript"></param>
        void LoadIllustration(EndingScript endingScript)
        {
            if (endingScript.HasIllust)
            {
                // 컷씬 들어가는 행
                GetImage((int)Images.IMG_Illustration).transform.SetSiblingIndex(1);
            }
            else
            {
                // 일러스트 다시 돌림
                GetImage((int)Images.IMG_Illustration).transform.SetSiblingIndex(0);
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
