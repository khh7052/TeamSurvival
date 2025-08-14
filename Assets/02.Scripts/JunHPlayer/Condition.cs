using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : MonoBehaviour
{
    public float curValue; //현재값
    public float startValue; //초기값
    public float maxValue; // 최댓값
    public float passiveValue; //소못값

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
