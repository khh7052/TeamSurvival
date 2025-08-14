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
    [SerializeField] Condition health; //체력
    [SerializeField] Condition hunger; //배고픔
    [SerializeField] Condition thirst; //목마름
    [SerializeField] Condition stamina; //스테미너

    [Header("이동관련")]
    public float moveSpeed; //이동속도
    public float jumpPower; //점프력

    public event Action OnChangeStatuses;

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
