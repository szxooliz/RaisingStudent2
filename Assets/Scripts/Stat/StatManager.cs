using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    // 스탯 최대값
    private const int MAX_STAT = 100;

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
        List<string> statListRemoved = statList.ToList();

        statListRemoved.Remove(statName); // 관련 스탯 삭제
        int increaseIdx = Random.Range(0, statListRemoved.Count); // 증가할 스탯의 인덱스 랜덤으로 구하기
        
        // 관련 스탯 증가
        switch(statName) {
            case "stamina":
                stamina += value;
                if (stamina > MAX_STAT) stamina = MAX_STAT;
                break;
            case "intelligence":
                intelligence += value;
                if (intelligence > MAX_STAT) intelligence = MAX_STAT;
                break;
            case "charisma":
                charisma += value;
                if (charisma > MAX_STAT) charisma = MAX_STAT;
                break;
            case "mania":
                mania += value;
                if (mania > MAX_STAT) mania = MAX_STAT;
                break;
        }
        // 랜덤 스탯 증가
        switch(statListRemoved[increaseIdx]) {
            case "stamina":
                stamina += value;
                if (stamina > MAX_STAT) stamina = MAX_STAT;
                break;
            case "intelligence":
                intelligence += value;
                if (intelligence > MAX_STAT) intelligence = MAX_STAT;
                break;
            case "charisma":
                charisma += value;
                if (charisma > MAX_STAT) charisma = MAX_STAT;
                break;
            case "mania":
                mania += value;
                if (mania > MAX_STAT) mania = MAX_STAT;
                break;
        }
        Debug.Log($"스탯 변화 - 체력: {stamina}, 지력: {intelligence}, 매력: {charisma}, 덕력: {mania}");
    }
}