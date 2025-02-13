using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    [System.Serializable]
    public class PlayerData
    {
        public string charName; // 현재 플레이 중인 캐릭터 이름
        public int currentTurn; // 턴 0~23
        public eMonths currentMonth; // n월 3-6/9-12
        public eThirds currentThird; // a순 상중하
        public List<ProcessData> watchedProcess; // 로그용 - 이미 진행한 or 진행 중인 활동, 이벤트 순서대로 저장
        public List<EventData> watchedEvents; // 이벤트 중복 실행 방지용 - 리스트에 있으면 이미 본 이벤트

        public event EventHandler OnStatusChanged; // 상태 변경에 따라 UI 활성화 하는 용도의 이벤트 핸들러

        [SerializeField] eStatus _currentStatus; // 현재 상태 
        public eStatus currentStatus
        {
            get => _currentStatus;
            set
            {
                _currentStatus = value;
                OnStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int[] statsAmounts; // 스탯 리스트, StatName 열거형 요소와 순서 같음

        [SerializeField] float _stressAmount; // 스트레스 양
        public float stressAmount
        {
            get => _stressAmount;
            set
            {
                _stressAmount = Mathf.Clamp(value, 0, 100);
            }
        }

        public PlayerData() // 생성자
        {
            charName = "Comsoon"; // 프로토타입
            currentTurn = 0;
            currentMonth = eMonths.Mar;
            currentThird = eThirds.First;
            currentStatus = eStatus.Main;

            statsAmounts = new int[4];
            for (int i = 0; i < statsAmounts.Length; i++)
            {
                statsAmounts[i] = 0; // 0으로 초기화
            }
            stressAmount = 0;
        }
    }

}