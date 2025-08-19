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
    /*
    public float moveSpeed; //�̵��ӵ�
    public float jumpPower; //������
    */
    public Status moveSpeed; // �̵� �ӵ�
    public Status jumpPower; // ������
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

    public IEnumerable<Condition> AllConditions //EntityModel�� Condition��ȸ ������Ƽ
    {
        get
        {
            yield return health;
            yield return hunger;
            yield return thirst;
            yield return stamina;
        }
    }
    private void ApplyPassiveValueCondition() // �ð��� �帧�� ���� ��ȭ�ϴ� ���� ���Ŀ� �µ� �ý��� ����� �̰��� �߰� ����
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
        //��� ����
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
    }
}
