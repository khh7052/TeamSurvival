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
    [SerializeField] Condition health; //ü��
    [SerializeField] Condition hunger; //�����
    [SerializeField] Condition thirst; //�񸶸�
    [SerializeField] Condition stamina; //���׹̳�

    [Header("�̵�����")]
    public float moveSpeed; //�̵��ӵ�
    public float jumpPower; //������

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
        //��� ����
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
    }
}
