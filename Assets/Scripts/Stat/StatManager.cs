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

    // 스탯 성공 확률
    private float statSuccessProb = 0;

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
    public void increaseStat(string statName, int statValue)
    {
        int stressValue = StressManager.instance.stressValue;
        statSuccessProb = DetermineSuccess(stressValue);
        statValue = (int)(statValue * statSuccessProb);

        List<string> statListRemoved = statList.ToList();
        statListRemoved.Remove(statName);
        int increaseIdx = Random.Range(0, statListRemoved.Count);
        
        // 관련 스탯 증가
        switch(statName) {
            case "stamina":
                stamina += statValue;
                // if (stamina > MAX_STAT) stamina = MAX_STAT;
                break;
            case "intelligence":
                intelligence += statValue;
                // if (intelligence > MAX_STAT) intelligence = MAX_STAT;
                break;
            case "charisma":
                charisma += statValue;
                // if (charisma > MAX_STAT) charisma = MAX_STAT;
                break;
            case "mania":
                mania += statValue;
                // if (mania > MAX_STAT) mania = MAX_STAT;
                break;
        }
        // 랜덤 스탯 증가
        switch(statListRemoved[increaseIdx]) {
            case "stamina":
                stamina += statValue;
                // if (stamina > MAX_STAT) stamina = MAX_STAT;
                break;
            case "intelligence":
                intelligence += statValue;
                // if (intelligence > MAX_STAT) intelligence = MAX_STAT;
                break;
            case "charisma":
                charisma += statValue;
                // if (charisma > MAX_STAT) charisma = MAX_STAT;
                break;
            case "mania":
                mania += statValue;
                // if (mania > MAX_STAT) mania = MAX_STAT;
                break;
        }
        Debug.Log($"스탯 - 체력: {stamina}, 지력: {intelligence}, 매력: {charisma}, 덕력: {mania}");
    }

    private float DetermineSuccess(int stressValue){
        int rand = Random.Range(0, 100);

        if (stressValue >= 70) {
            if (rand < 50)  // 대실패 50%
                return 0.5f;
            else if (rand < 95)  // 성공 45%
                return 1.0f;
            else                 // 대성공 5%
                return 1.5f;
        }
        else if (stressValue >= 40) {
            if (rand < 80)  // 성공 80%
                return 1.0f;
            else                 // 대성공 20%
                return 1.5f;
        }
        else if (stressValue >= 0) {
            if (rand < 60)  // 성공 60%
                return 1.0f;
            else  // 대성공 40%
                return 1.5f;
        }
        else
            return 0.0f;
    }
}
