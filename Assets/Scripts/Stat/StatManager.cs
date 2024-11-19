using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager instance;

    // 기본 스탯 변수들
    public int stamina; // 체력
    public int intelligence; // 지력
    public int charisma; // 매력
    public int mania; //덕력

    // 기본 스탯 리스트
    private List<string> statList = new List<string> {"stamina", "intelligence", "charisma", "mania"};

    // 스탯 최대, 최소 값
    private const int MAX_STAT = 100;
    private const int MIN_STAT = 100;

    void Awake()
    {
        // 싱글톤 패턴 적용
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 버튼 클릭 시 관련 스탯과 랜덤 스탯 증가
    public void increaseStat(string statName, int value)
    {
        // 스탯 리스트 복사
        List<string> statListRemoved = new List<string>();
        // statListRemoved = statList.ToList();

        // 랜덤 스탯 선정
        

    }
}

// todo : 스탯 증가 감소 구현
// 버튼 누르면 해당 스탯과 연관된 다른 스탯 중 하나가 랜덤으로 증가