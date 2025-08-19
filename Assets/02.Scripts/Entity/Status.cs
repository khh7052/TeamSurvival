using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Status
{
    [SerializeField] private float baseValue;       // �⺻ ��ġ
    [SerializeField] private float additionalValue; // ���� ����
    [SerializeField] private float multipleValue = 1f;  // ���� ��ġ
    [SerializeField] private float constValue = 0f; // ��� ��ġ (���� ������)
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
