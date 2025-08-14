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
    public float moveSpeed; //이동속도
    public float jumpPower; //점프력

    public event Action OnChangeStatuses;

    private void Awake()
    {
        health.Init();
        hunger.Init();
        thirst.Init();
        stamina.Init();

        health.OnChanged += () => OnChangeStatuses?.Invoke();
        hunger.OnChanged += () => OnChangeStatuses?.Invoke();
        thirst.OnChanged += () => OnChangeStatuses?.Invoke();
        stamina.OnChanged += () => OnChangeStatuses?.Invoke();
    }

    private void Update()
    {
        hunger.Subtract(hunger.PassiveValue * Time.deltaTime);
        stamina.Add(stamina.PassiveValue * Time.deltaTime);
    }

    public void Heal(float amount)
    {
        health.Add(amount);
        OnChangeStatuses?.Invoke();
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
        OnChangeStatuses?.Invoke();
    }

    public void Drink(float amount)
    {
        thirst.Add(amount);
        OnChangeStatuses?.Invoke();
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
