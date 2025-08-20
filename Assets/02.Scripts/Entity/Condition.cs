using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Condition
{
    [SerializeField] private float curValue; //���簪
    [SerializeField] private float startValue; //�ʱⰪ
    [SerializeField] private float maxValue; // �ִ�
    [SerializeField] private float passiveValue; //�Ҹ���

    public float CurValue => curValue;
    public float MaxValue => maxValue;
    public float PassiveValue => passiveValue;

    public event Action OnChanged;

    public void Init() //�����ʱ�ȭ �Լ�
    {
        curValue = startValue;
//        OnChanged?.Invoke(); //���º��� �˸�
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
 