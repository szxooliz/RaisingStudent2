using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour
{
    private void Awake()
    {
        GameManager instance = GameManager.Instance;
    }

    private void Start()
    {
        UI_Manager.Instance.ShowSceneUI<UI_BaseSceneBtn>();
        UI_Manager.Instance.ShowSceneUI<UI_BaseSceneTxt>();

    }
}
