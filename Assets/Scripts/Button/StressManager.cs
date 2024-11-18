using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressManager : MonoBehaviour
{
    public static StressManager instance; // 싱글톤 패턴 적용

    public int stressValue = 0; // 스트레스 전역 변수 선언
    public int maxStressValue = 100; // 최대 스트레스 값 선언
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) 
        { 
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    public void increaseStress(int value) // 스트레스 증가 함수
    {
        stressValue += value;
        if (stressValue > maxStressValue)
        {
            stressValue = maxStressValue;
        }
        Debug.Log("Stress: " + stressValue);
    }

    public void decreaseStress(int value) // 스트레스 감소 함수
    {
        stressValue -= value;
        if (stressValue < 0)
        {
            stressValue = 0;
        }
        Debug.Log("Stress: " + stressValue);
    }
}
