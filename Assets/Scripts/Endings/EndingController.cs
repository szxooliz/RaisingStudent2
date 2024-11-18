using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// 테스트를 위한 가상의 능력치
public class PlayerStats
{
    public int stamina; // 체력
    public int intelligence; // 지력
    public int charisma; // 매력
    public int mania; // 덕력

    public PlayerStats(int stamina, int intelligence, int charisma, int mania)
    {
        this.stamina = stamina;
        this.intelligence = intelligence;
        this.charisma = charisma;
        this.mania = mania;
    }

    // 모든 능력치 리스트로 반환
    public List<int> GetAllStats()
    {
        return new List<int> { stamina, intelligence, charisma, mania };
    }
}

public class EndingController : MonoBehaviour
{
    PlayerStats mockPlayerStats = new PlayerStats(80, 30, 20, 80); // 테스트를 위한 가상의 능력

    private void Start() // 테스트를 위한 구문
    {
        EndGame();
    }

    public void EndGame() // 게임 종료 시점에 호출
    {
        CheckEnding();
    }

    void CheckEnding()
    {
        // 80 이상인 능력치 개수 계산
        int highStatsCount = mockPlayerStats.GetAllStats().Count(stat => stat >= 80);

        if (highStatsCount == 4)
        {
            Debug.Log("대학원 엔딩");
            // SceneManager.LoadScene("GraduateEndingScene");
        }
        else if (highStatsCount >= 2)
        {
            if (mockPlayerStats.intelligence >= 80 && mockPlayerStats.charisma >= 80)
            {
                Debug.Log("대기업 SI 취업 엔딩");
            }
            else if (mockPlayerStats.intelligence >= 80 && mockPlayerStats.mania >= 80)
            {
                Debug.Log("게임회사 취업 엔딩");
            }
            else if (mockPlayerStats.charisma >= 80 && mockPlayerStats.mania >= 80)
            {
                Debug.Log("버튜버 엔딩");
            }
            else if(mockPlayerStats.stamina >= 80 && mockPlayerStats.mania >= 80)
            {
                Debug.Log("프로게이머 엔딩");
            }
        }
        else
        {
            Debug.Log("홈프로텍터 엔딩");
        }
    }
}
