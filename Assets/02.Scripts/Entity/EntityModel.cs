using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IDamageable //���ع����� �ִ���
{
    void TakePhysicalDamage(int damage);
    Action OnHitEvent {  get; }
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

    public float warningTemp = 34f; //���
    public float dangerTemp = 32f; //���� 
    
    private float time = 0f;
    private float interval = 1f; //������ ���� ü�� ���� ���ʿ� �ѹ� ����������
    [SerializeField] private float rayLength = 5f; //�׽�Ʈ�� ��������
    public Action OnHitEvent { get; set; }


    [Header("�̵�����")]
    /*
    public float moveSpeed; //�̵��ӵ�
    public float jumpPower; //������
    */
    public Status moveSpeed; // �̵� �ӵ�
    public Status jumpPower; // ������
    //public event Action OnChangeStatuses;

    private WeatherType currentWeather;

    [Header("���� ������")]
    [SerializeField] private float starvationDps = 0.25f;
    [SerializeField] private float dehydrationDps = 0.25f;

    [Header("�ǰ� �� ó�� ������")]
    [SerializeField] MeshRenderer mesh;
    private Coroutine coroutine;

    private void Awake()
    {
        foreach (var condition in AllConditions)
        {
            condition.Init();
        }
    }

    private void OnEnable()
    {
        if (WeatherCycle.Instance != null)
            WeatherCycle.Instance.RegisterObserver(this);
        OnHitEvent += OnHit;
    }

    private void OnDisable()
    {
        OnHitEvent -= OnHit;
    }

    private void Update()
    {
        ApplyPassiveValueCondition();
        DamageByNeeds();

        if (isApplyByWeather)
        {
            UpdateTemperture();
            DamageByTemperature();
        }
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
        if (hunger.PassiveValue != 0)
            hunger.Subtract(hunger.PassiveValue * Time.deltaTime);
        if (stamina.PassiveValue != 0)
            stamina.Add(stamina.PassiveValue * Time.deltaTime);
        if (thirst.PassiveValue != 0)
            thirst.Subtract(thirst.PassiveValue * Time.deltaTime);

        //�����, ���� 0�� �� ü�� ��� ����
        bool isDeprived = (hunger.CurValue <= 0f) || (thirst.CurValue <= 0f);
        if (!isDeprived && health.PassiveValue != 0)
            health.Add(health.PassiveValue * Time.deltaTime);
    }

    private void DamageByNeeds()
    {
        float dps = 0f;

        if (hunger.CurValue <= 0f) dps += starvationDps;
        if (thirst.CurValue <= 0f) dps += dehydrationDps;

        if (dps > 0f && health.CurValue > 0f)
            health.Subtract(dps * Time.deltaTime);
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
        OnHitEvent?.Invoke();
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
            //Debug.Log("���� �ǳ� or ���� �����־� ������ ������ �����ʽ��ϴ�. ü���� �ڿ�ȸ���˴ϴ�.");
            if(temperture.CurValue <= 36.5f)
            {
                temperture.Add(0.05f);
                //Debug.Log($"ü��ȸ��{temperture.CurValue}");
            }
            else if (temperture.CurValue >= 36.5f)
            {
                //ü���� �ö󰡴� ���� �߰��� ex ) Heat �ö� ü���� 36.5���� �����ִ� �κ�
            }
            return;
        }


        float decreaseAmount = GetWeatherTempertureDecrease(currentWeather);
        if (decreaseAmount > 0f)
        {
            temperture.Subtract(decreaseAmount);
            //Debug.Log($"���� ü�� : {this.temperture.CurValue} ���ҷ� : {decreaseAmount}");
        }
    }

    private void DamageByTemperature()
    {
        float temp = temperture.CurValue;
        float dmg = 0f;

        if(temp <= dangerTemp)
        {
            dmg = 0.2f;
        }
        else if(temp <= warningTemp)
        {
            dmg = 0.05f;
        }
        else
        {
            dmg = 0f;
        }

        if(dmg > 0f)
        {
            health.Subtract(dmg);
        }
    }

    private bool IsInside() //�ǳ����� üũ���ִ� �Լ�
    {
        Vector3 ray = transform.position + Vector3.up * 1.8f;
        bool isHit = Physics.Raycast(ray, Vector3.up, rayLength, ~0);

        Debug.DrawRay(ray, Vector3.up * rayLength, isHit ? Color.green : Color.red);

        return isHit;
    }


    public void OnHit()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(SetMeshColorAtDamage());
    }

    private IEnumerator SetMeshColorAtDamage()
    {
        Color startColor = Color.red;       // ���� �� (����)
        Color endColor = Color.white;       // ���� �� (�Ͼ�)
        float duration = 1.0f;              // ȿ�� ���� �ð� (1��)
        float elapsed = 0f;


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; // 0 �� 1 �� ����
            mesh.material.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        // ����: ���������� ��� Ȯ��
        mesh.material.color = endColor;
    }
}

