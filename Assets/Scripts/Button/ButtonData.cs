using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonData", menuName = "Buttons/ButtonData", order = 1)]
public class ButtonData : ScriptableObject
{
    public string buttonName; // button의 이름
    public int stressValue; // 조정할 스트레스 값
    public ButtonType buttonType; // increase인지 decrease인지 나타냄
}

public enum ButtonType
{
    Increase,
    Decrease
}
