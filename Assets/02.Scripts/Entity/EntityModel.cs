using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable //피해받을수 있는지
{
    void TakePhysicalDamage(int damage);
}
public class EntityModel : MonoBehaviour, IDamageable
{
    [Header("상태관련")]
    public Condition health; //체력
    public Condition hunger; //배고픔
    public Condition thirst; //목마름
    public Condition stamina; //스테미너

    [Header("이동관련")]
    /*
    public float moveSpeed; //이동속도
    public float jumpPower; //점프력
    */
    public Status moveSpeed; // 이동 속도
    public Status jumpPower; // 점프력
    //public event Action OnChangeStatuses;

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
    }

    public IEnumerable<Condition> AllConditions //EntityModel의 Condition순회 프로퍼티
    {
        get
        {
            yield return health;
            yield return hunger;
            yield return thirst;
            yield return stamina;
        }
    }
    private void ApplyPassiveValueCondition() // 시간이 흐름에 따라 변화하는 스탯 이후에 온도 시스템 생기면 이곳에 추가 가능
    {
        hunger.Subtract(hunger.PassiveValue * Time.deltaTime);
        stamina.Add(stamina.PassiveValue * Time.deltaTime);
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
}
