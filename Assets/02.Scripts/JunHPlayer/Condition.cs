using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : MonoBehaviour
{
    public float curValue; //���簪
    public float startValue; //�ʱⰪ
    public float maxValue; // �ִ�
    public float passiveValue; //�Ҹ���

    private void Start()
    {
        curValue = startValue;
    }

    private void Update()
    {
        //UI Update
    }

    float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value, maxValue);
    }

    public void Subtract(float value)
    {
        curValue = Mathf.Max(curValue - value, 0);
    }


}
