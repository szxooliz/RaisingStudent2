using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class MainUI : MonoBehaviour
    {
        [SerializeField] Image charImage;

        private void OnEnable()
        {
            // 계절 변화마다 스프라이트 변경이 늦음
            // 스프라이트 변경이 conversation에 묶여있어서
            // 일단 그냥 기본적으로 옷만 바꾸게..

            string path = Util.GetSeasonIllustPath("basic");
            charImage.sprite = DataManager.Instance.GetOrLoadSprite(path);
        }
    }

}
