using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable //���ع����� �ִ���
{
    void TakePhysicalDamage(int damage);
}

public interface IWeatherObserver
{
    void OnWeatherChanged(WeatherType newWeather);
}

public class EntityModel : MonoBehaviour, IDamageable, IWeatherObserver

{
    [Header("���°���")]
    public Condition health; //ü��
    public Condition hunger; //�����
    public Condition thirst; //�񸶸�
    public Condition stamina; //���׹̳�
    public Condition temperture; //ü��
    public bool isApplyByWeather; //ü�¿� ������ �޴°�



    [Header("�̵�����")]
    /*
    public float moveSpeed; //�̵��ӵ�
    public float jumpPower; //������
    */
    public Status moveSpeed; // �̵� �ӵ�
    public Status jumpPower; // ������
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

    public IEnumerable<Condition> AllConditions //EntityModel�� Condition��ȸ ������Ƽ
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
    private void ApplyPassiveValueCondition() // �ð��� �帧�� ���� ��ȭ�ϴ� ���� ���Ŀ� �µ� �ý��� ����� �̰��� �߰� ����
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
        //��� ����
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
        Debug.Log($"{gameObject.name} �� ������ {newWeather}�� �ٲ�");
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
            Debug.Log($"{gameObject.name} ü�� ������: {decreaseAmount * Time.deltaTime}");
        }
    }
}
