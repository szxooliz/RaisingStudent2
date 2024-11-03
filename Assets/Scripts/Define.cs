using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Define
{
    public enum Scene
    {

    }

    public enum Sound
    {
        Effect, Bgm,
        MaxCount
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum Terms
    {
        Mar, Apr, May, Jun,   
        Sep, Oct, Nov, Dec

    }

    public enum Turns
    {
        First, Second, Third
    }
    

    #region StatName
    public enum StatName
    {
        Inteli, Otaku, Strength, Charming
    }
    public enum StatNameKor
    {
        지력, 덕력, 체력, 매력
    }

    public static string GetStatNameKor(StatName statName)
    {
        string temp = "";
        temp += ((StatNameKor)((int)statName)).ToString();
        return temp;
    }
    #endregion


    public enum ActivityType
    {
        Class, Game, Workout, Club, Rest
    }

    public enum EventDataType
    {
        Main, Sub, Conditioned
    }

    public enum EventType
    {
        SummerVac, WinterVac
    }

    public enum EndingName
    {

    }

    public class PlayerData
    {
        public int currentTerm;
        public int currentTurn;
        public float[] statsAmounts;
        public float stressAmount
        {
            get => stressAmount;
            set
            {
                stressAmount = Mathf.Clamp(value, 0, 100);

            }
        }

        public PlayerData()
        {
            currentTerm = (int)Terms.Mar;
            currentTurn = (int)Turns.First;
            statsAmounts = new float[4];
            stressAmount = 0;
        }
    }

}