using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IDamageable //피해받을수 있는지
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
    [Header("상태관련")]
    public Condition health; //체력
    public Condition hunger; //배고픔
    public Condition thirst; //목마름
    public Condition stamina; //스테미너
    public Condition temperture; //체온
    public bool isApplyByWeather; //날씨에 체온이 영향을 받는가

    public float warningTemp = 34f; //경고
    public float dangerTemp = 32f; //위험 
    
    private float time = 0f;
    private float interval = 1f; //날씨에 대한 체온 영향 몇초에 한번 받을것인지
    [SerializeField] private float rayLength = 5f; //테스트용 삭제가능
    public Action OnHitEvent { get; set; }


    [Header("이동관련")]
    /*
    public float moveSpeed; //이동속도
    public float jumpPower; //점프력
    */
    public Status moveSpeed; // 이동 속도
    public Status jumpPower; // 점프력
    //public event Action OnChangeStatuses;

    private WeatherType currentWeather;

    [Header("결핍 데미지")]
    [SerializeField] private float starvationDps = 0.25f;
    [SerializeField] private float dehydrationDps = 0.25f;

    [Header("피격 시 처리 렌더러")]
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
        if (hunger.PassiveValue != 0)
            hunger.Subtract(hunger.PassiveValue * Time.deltaTime);
        if (stamina.PassiveValue != 0)
            stamina.Add(stamina.PassiveValue * Time.deltaTime);
        if (thirst.PassiveValue != 0)
            thirst.Subtract(thirst.PassiveValue * Time.deltaTime);

        //배고픔, 수분 0일 때 체력 재생 금지
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
        //사망 로직
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        OnHitEvent?.Invoke();
    }

    public void OnWeatherChanged(WeatherType newWeather) //날씨 바뀔때 호출되는 함수(옵저버)
    {
        currentWeather = newWeather;
        Debug.Log($"{gameObject.name} → 날씨가 {newWeather}로 바뀜");
    }

    private float GetWeatherTempertureDecrease(WeatherType weather) //체온 감소량 반환 함수
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

    private void UpdateTemperture() //체온 감소 로직
    {
        if (!isApplyByWeather) return; //날씨에 영향을 받는 대상인지?

        bool isGood = IsInside() || currentWeather == WeatherType.Clear; //Ray를 time에 영향받지 않고 계속 쏴주려고 추가했습니다 수정가능

        time += Time.deltaTime;
        if (time < interval) return;
        time = 0f;

        if (isGood) // 실내인지 or 날씨가 맑은지  
        {
            //Debug.Log("현재 실내 or 무언가 위에있어 날씨의 영향을 받지않습니다. 체온이 자연회복됩니다.");
            if(temperture.CurValue <= 36.5f)
            {
                temperture.Add(0.05f);
                //Debug.Log($"체온회복{temperture.CurValue}");
            }
            else if (temperture.CurValue >= 36.5f)
            {
                //체온이 올라가는 날씨 추가시 ex ) Heat 올라간 체온을 36.5까지 맞춰주는 부분
            }
            return;
        }


        float decreaseAmount = GetWeatherTempertureDecrease(currentWeather);
        if (decreaseAmount > 0f)
        {
            temperture.Subtract(decreaseAmount);
            //Debug.Log($"현재 체온 : {this.temperture.CurValue} 감소량 : {decreaseAmount}");
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

    private bool IsInside() //실내인지 체크해주는 함수
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
        Color startColor = Color.red;       // 시작 색 (빨강)
        Color endColor = Color.white;       // 최종 색 (하양)
        float duration = 1.0f;              // 효과 지속 시간 (1초)
        float elapsed = 0f;


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; // 0 → 1 로 증가
            mesh.material.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        // 보정: 최종적으로 흰색 확정
        mesh.material.color = endColor;
    }
}

