using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Status
{
    [SerializeField] private float baseValue;       // 기본 수치
    [SerializeField] private float additionalValue; // 증가 수차
    [SerializeField] private float multipleValue = 1f;  // 배율 수치
    [SerializeField] private float constValue = 0f; // 상수 수치 (배율 미적용)
    [SerializeField] public float totalValue => (baseValue + additionalValue) * multipleValue + constValue;

    public Action OnChangeValue;

    public void AddAdditionalValue(float value)
    {
        additionalValue += value;
        OnChangeValue?.Invoke();
    }

    public void AddMultiplyValue(float value) 
    { 
        multipleValue += value; 
        OnChangeValue?.Invoke();
    }

    public void AddConstValue(float value) { constValue += value; }
}
