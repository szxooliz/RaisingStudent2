using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EndingData", menuName = "Game/EndingData")]
public class EndingData : ScriptableObject
{
    public string endingName;       // 엔딩 이름
    public bool isUnlocked;         // 해금 여부
    public string applicationField; // 지원 분야
    public string grade;            // 게임 성적
    public string awards;           // 기타 이력
}
