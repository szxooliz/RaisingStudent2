using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class Util
    {
        public static bool nowTexting = false;                                                                                                                                                                      
        /// <summary>
        /// Game Object에서 해당 Component 얻거나 없으면 추가 (주의 무거움)
        /// </summary>
        public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
        {
            return go.GetComponent<T>() ?? go.AddComponent<T>();
        }

        /// <summary>
        /// go의 자식 오브젝트로부터 T 형식의 컴포넌트 찾는 함수
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="name"></param>
        /// <param name="recursive">true: 전체 자식에서, false:자식에서 제일 먼저 보이는 컴포넌트만</param>
        /// <returns></returns>
        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
        {
            if (go == null)
                return null;

            if (recursive == false)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform transform = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        T component = transform.GetComponent<T>();
                        if (component != null)
                            return component;
                    }
                }
            }
            else
            {
                foreach (T component in go.GetComponentsInChildren<T>())
                {
                    if (string.IsNullOrEmpty(name) || component.name == name)
                    {
                        return component;
                    }
                }
            }

            return null;
        }

        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            Transform transform = FindChild<Transform>(go, name, recursive);
            if (transform == null)
                return null;

            return transform.gameObject;
        }

        public static IEnumerator LoadTextOneByOne(string inputTextString, TMPro.TMP_Text inputTextUI, float eachTime = 0.05f, bool canClickSkip = true)
        {
            nowTexting = true;
            float miniTimer = 0f; //타이머
            float currentTargetNumber = 0f; // 해당 Time에 출력을 목표로 하는 최소 글자 수
            int currentNumber = 0; // 해당 Time에 출력중인 글자 수
            string displayedText = "";
            StringBuilder builder = new StringBuilder(displayedText);
            while (currentTargetNumber < inputTextString.Length)
            {
                while (currentNumber < currentTargetNumber)
                { // 목표 글자수까지 출력
                  //displayedText += inputTextString.Substring(currentNumber,1);
                    builder.Append(inputTextString.Substring(currentNumber, 1));
                    currentNumber++;
                }
                //inputTextUI.text = displayedText;
                inputTextUI.text = builder.ToString();
                yield return null;
                miniTimer += Time.deltaTime;
                currentTargetNumber = miniTimer / eachTime;
                if ((Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Space)) && canClickSkip)
                {
                    break;
                }
            }
            while (currentNumber < inputTextString.Length)
            { // 목표 글자수까지 출력
                builder.Append(inputTextString.Substring(currentNumber, 1));
                currentNumber++;
            }
            inputTextUI.text = builder.ToString();
            yield return null;
            nowTexting = false;
        }

        #region 일러스트 경로 반환 함수 오버로드
        /// <summary>
        /// 계절에 맞는 옷 일러스트 경로 반환 - 활동 결과
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static string GetSeasonIllustPath(string face)
        {
            string str = "";

            // 5월 상순(7턴) ~ 10월 하순(18턴) - 반팔 이미지
            if (DataManager.Instance.playerData.CurrentTurn >= 7 && DataManager.Instance.playerData.CurrentTurn <= 18)
            {
                str = $"Sprites/Character/{DataManager.Instance.playerData.CharName}_Summer_{face}";
            }
            else
            {
                str = $"Sprites/Character/{DataManager.Instance.playerData.CharName}_{face}";
            }

            return str;
        }

        /// <summary>
        /// 계절에 맞는 옷 일러스트 경로 반환
        /// </summary>
        /// <param name="script">대화 인터랙션 스크립트</param>
        /// <returns></returns>
        public static string GetSeasonIllustPath(Script script)
        {
            string str = "";
            
            // 계절 이미지가 있는 플레이어블 캐릭터의 경우
            // 5월 상순(7턴) ~ 10월 하순(17턴) - 반팔 이미지
            if (DataManager.Instance.playerData.CurrentTurn >= 7 && DataManager.Instance.playerData.CurrentTurn <= 18)
            {
                str = $"Sprites/Character/{script.Character}_Summer_{script.Face}";
            }
            else
            {
                str = $"Sprites/Character/{script.Character}_{script.Face}";
            }

            return str;
        }

        /// <summary>
        /// 계절에 맞는 옷 일러스트 경로 반환
        /// </summary>
        /// <param name="eventScript">메인 이벤트 스크립트</param>
        /// <returns></returns>
        public static string GetSeasonIllustPath(EventScript eventScript)
        {
            string str = "";

            // 계절 이미지가 없는 캐릭터의 경우
            if (eventScript.Character != DataManager.Instance.playerData.CharName)
            {
                string charSprite = DataManager.Instance.CharFaceDict[eventScript.Character].basic;
                if (charSprite == "none") return null;

                str = $"Sprites/Character/{charSprite}";
                return str;
            }

            // 계절 이미지가 있는 플레이어블 캐릭터의 경우
            // 5월 상순(7턴) ~ 10월 하순(18턴) - 반팔 이미지
            if (DataManager.Instance.playerData.CurrentTurn >= 7 && DataManager.Instance.playerData.CurrentTurn <= 18)
            {
                str = $"Sprites/Character/{eventScript.Character}_Summer_{eventScript.Face}";
            }
            else
            {
                str = $"Sprites/Character/{eventScript.Character}_{eventScript.Face}";
            }

            return str;
        }

        #endregion


        public static bool TryFindTuple(List<(long, bool)> list, long key, out (long, bool) result)
        {
            result = list.FirstOrDefault(item => item.Item1 == key);
            return result.Item1 == key; // key가 0일 경우 대비
        }

        /// <summary>
        /// 캐릭터 한국어 이름 가져오기
        /// </summary>
        public static string GetCharNameKor(string _charName)
        {
            if (!DataManager.Instance.CharFaceDict.ContainsKey(_charName))
            {
                Debug.LogError($"캐릭터 고유값 오류, key 없음");
                return null;
            }
            return DataManager.Instance.CharFaceDict[_charName].CharacterName;
        }

        /// <summary>
        /// 캐릭터 이미지 기본 path
        /// </summary>
        public static string GetCharBasicSpritePath(string _charName)
        {
            if (!DataManager.Instance.CharFaceDict.ContainsKey(_charName))
            {
                Debug.LogError($"캐릭터 고유값 오류, key 없음");
                return null;
            }
            return DataManager.Instance.CharFaceDict[_charName].basic;
        }

        public static string GetActivityTitle(SystemEnum.eActivityType eActivity)
        {
            StringBuilder sb = new();
            sb.Append($"{GetActivityTypeKor(eActivity)}");
            if (eActivity == eActivityType.Class)
                sb.Append("을 들었다!");
            else if (eActivity == eActivityType.Club)
                sb.Append("활동을 했다!");
            else sb.Append("을 했다!");

            return sb.ToString();
        }
    }
}
