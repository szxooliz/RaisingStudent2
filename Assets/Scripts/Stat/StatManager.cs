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
    public void increaseStat(string statName, int statValue)
    {
        int stressValue = StressManager.instance.stressValue; // 스트레스 변수
        bool isFailure = false; // 실패 여부 변수

        if (stressValue >= 70) { // 스트레스가 70 이상이라면
            if (Random.value < 0.5f) { // 10% 확률로 실패
                isFailure = true;
            }
            statValue /= 2; // 스탯 값 상승량 절반
        }

        if (!isFailure) { // 실패하지 않았다면
            List<string> statListRemoved = statList.ToList();

            statListRemoved.Remove(statName); // 관련 스탯 삭제
            int increaseIdx = Random.Range(0, statListRemoved.Count); // 증가할 스탯의 인덱스 랜덤으로 구하기
            
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
        }
        else {
            Debug.Log("실패");
        }
        Debug.Log($"스탯 - 체력: {stamina}, 지력: {intelligence}, 매력: {charisma}, 덕력: {mania}, failure: {isFailure}");
    }
}