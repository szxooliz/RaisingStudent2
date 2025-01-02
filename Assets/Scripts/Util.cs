using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    }
}
