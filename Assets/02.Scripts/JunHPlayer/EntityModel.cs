using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable //���ع����� �ִ���
{
    void TakePhysicalDamage(int damage);
}
public class EntityModel : MonoBehaviour, IDamageable
{
    [Header("���°���")]
    public Condition health; //ü��
    public Condition hunger; //�����
    public Condition thirst; //�񸶸�
    public Condition stamina; //���׹̳�

    [Header("�̵�����")]
    public float moveSpeed; //�̵��ӵ�
    public float jumpPower; //������

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
        //��� ����
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
    }
}
