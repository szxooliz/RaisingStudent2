using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    public EndingData[] endings; // 모든 엔딩 데이터

    public static EndingManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public EndingData[] GetUnlockedEndings()
    {
        // 해금된 엔딩만 반환
        return System.Array.FindAll(endings, ending => ending.isUnlocked);
    }
}

