using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable //피해받을수 있는지
{
    void TakePhysicalDamage(int damage);
}

public interface IWeatherObserver
{
    void OnWeatherChanged(WeatherType newWeather);
}

public class EntityModel : MonoBehaviour, IDamageable, IWeatherObserver

{
    [Header("상태관련")]
    public Condition health; //체력
    public Condition hunger; //배고픔
    public Condition thirst; //목마름
    public Condition stamina; //스테미너
    public Condition temperture; //체온
    public bool isApplyByWeather; //체온에 영향을 받는가



    [Header("이동관련")]
    /*
    public float moveSpeed; //이동속도
    public float jumpPower; //점프력
    */
    public Status moveSpeed; // 이동 속도
    public Status jumpPower; // 점프력
    //public event Action OnChangeStatuses;

    private WeatherType currentWeather;


    private void Awake()
    {
        foreach(var condition in AllConditions)
        {
            condition.Init();
        }
    }

    private void Update()
    {
        ApplyPassiveValueCondition();
        TestCheck();
    }

    public IEnumerable<Condition> AllConditions //EntityModel의 Condition순회 프로퍼티
    {
        get
        {
            yield return health;
            yield return hunger;
            yield return thirst;
            yield return stamina;

            yield return temperture;
        }
    }
    private void ApplyPassiveValueCondition() // 시간이 흐름에 따라 변화하는 스탯 이후에 온도 시스템 생기면 이곳에 추가 가능
    {
        if(hunger.PassiveValue != 0)
            hunger.Subtract(hunger.PassiveValue * Time.deltaTime);
        if(stamina.PassiveValue != 0)
            stamina.Add(stamina.PassiveValue * Time.deltaTime);
        if(thirst.PassiveValue != 0)
            thirst.Subtract(thirst.PassiveValue * Time.deltaTime);
        if(health.PassiveValue != 0)
            health.Add(health.PassiveValue * Time.deltaTime);
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public void Drink(float amount)
    {
        thirst.Add(amount);
    }

    public void Die()
    {
        //사망 로직
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
    }

    private void OnEnable()
    {
        if (WeatherCycle.Instance != null)
            WeatherCycle.Instance.RegisterObserver(this);
    }


    public void OnWeatherChanged(WeatherType newWeather)
    {
        currentWeather = newWeather;
        Debug.Log($"{gameObject.name} → 날씨가 {newWeather}로 바뀜");
    }

    private float GetWeatherTemperatureDecrease(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Rain:
                return 0.01f;
            case WeatherType.Snow:
                return 0.05f;
            default:
                return 0f;
        }
    }

    private void TestCheck()
    {
        if (!isApplyByWeather) return;

        float decreaseAmount = GetWeatherTemperatureDecrease(currentWeather);
        if (decreaseAmount > 0f)
        {
            temperture.Subtract(decreaseAmount);
            Debug.Log($"{gameObject.name} 체온 감소중: {decreaseAmount * Time.deltaTime}");
        }
    }
}
