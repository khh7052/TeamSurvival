using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    public bool isApplyByWeather; //������ ü���� ������ �޴°�

    private float time = 0f;
    private float interval = 1f; //������ ���� ü�� ���� ���ʿ� �ѹ� ����������
    [SerializeField]private float rayLength = 5f; //�׽�Ʈ�� ��������



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

    private void Start()
    {
        if (WeatherCycle.Instance != null)
            WeatherCycle.Instance.RegisterObserver(this);
    }

    private void Update()
    {
        ApplyPassiveValueCondition();
        UpdateTemperture();
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

    public void OnWeatherChanged(WeatherType newWeather) //���� �ٲ� ȣ��Ǵ� �Լ�(������)
    {
        currentWeather = newWeather;
        Debug.Log($"{gameObject.name} �� ������ {newWeather}�� �ٲ�");
    }

    private float GetWeatherTempertureDecrease(WeatherType weather) //ü�� ���ҷ� ��ȯ �Լ�
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

    private void UpdateTemperture() //ü�� ���� ����
    {
        if (!isApplyByWeather) return; //������ ������ �޴� �������?

        bool isGood = IsInside() || currentWeather == WeatherType.Clear; //Ray�� time�� ������� �ʰ� ��� ���ַ��� �߰��߽��ϴ� ��������

        time += Time.deltaTime;
        if (time < interval) return;
        time = 0f;

        if (isGood) // �ǳ����� or ������ ������
        {
            Debug.Log("���� �ǳ� or ���� �����־� ������ ������ �����ʽ��ϴ�. ü���� �ڿ�ȸ���˴ϴ�.");
            if(temperture.CurValue <= 36.5f)
            {
                temperture.Add(0.05f);
                Debug.Log($"ü��ȸ��{temperture.CurValue}");
            }
            else if(temperture.CurValue >= 36.5f)
            {
                //ü���� �ö󰡴� ���� �߰��� ex ) Heat �ö� ü���� 36.5���� �����ִ� �κ�
            }
                return; 
        }
        

        float decreaseAmount = GetWeatherTempertureDecrease(currentWeather);
        if (decreaseAmount > 0f)
        {
            temperture.Subtract(decreaseAmount);
            Debug.Log($"���� ü�� : {this.temperture.CurValue} ���ҷ� : {decreaseAmount}");
        }
    }

    private bool IsInside() //�ǳ����� üũ���ִ� �Լ�
    {
        Vector3 ray = transform.position + Vector3.up * 1.8f;
        bool isHit = Physics.Raycast(ray, Vector3.up, rayLength, ~0);

        Debug.DrawRay(ray, Vector3.up * rayLength, isHit ? Color.green : Color.red);

        return isHit;
    }
}

