using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class BaseScene : MonoBehaviour
    {
        private void Awake()
        {
            GameManager instance = GameManager.Instance;
            UI_Manager.Instance.Clear();
        }

        private void Start()
        {
            UI_Manager.Instance.ShowSceneUI<UI_BaseScene>();
        }
    }

}
