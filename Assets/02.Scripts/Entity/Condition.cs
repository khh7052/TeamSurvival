using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Condition
{
    [SerializeField] private float curValue; //현재값
    [SerializeField] private float startValue; //초기값
    [SerializeField] private float maxValue; // 최댓값
    [SerializeField] private float passiveValue; //소못값

    public float CurValue => curValue;
    public float MaxValue => maxValue;
    public float PassiveValue => passiveValue;

    public event Action OnChanged;

    public void Init() //상태초기화 함수
    {
        curValue = startValue;
//        OnChanged?.Invoke(); //상태변경 알림
    }

    public float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value, maxValue);
        OnChanged?.Invoke();
    }

    public void Subtract(float value)
    {
        curValue = Mathf.Max(curValue - value, 0);
        OnChanged?.Invoke();
    }
}
 