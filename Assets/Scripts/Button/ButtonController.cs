using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public ButtonData buttonData;

    public void OnButtonClick()
    {
        if (buttonData == null)
        {
            Debug.LogError("ButtonData is null!");
            return;
        }
        // 디버그용 로그 추가
        Debug.Log($"Button clicked! Type: {buttonData.buttonType}, Value: {buttonData.stressValue}");
        
        // 버튼 클릭 시 스트레스 증가 또는 감소
        if (buttonData.buttonType == ButtonType.Increase)
        {
            StressManager.instance.increaseStress(buttonData.stressValue);
        }
        else if (buttonData.buttonType == ButtonType.Decrease)
        {
            StressManager.instance.decreaseStress(buttonData.stressValue);
        }

        // 자체휴강 버튼이 아니라면 스탯 증가(임의의 증가량 30)
        if (buttonData.buttonName != "SelfRest") {
            StatManager.instance.increaseStat(buttonData.buttonName, 30);
        }
    }
}