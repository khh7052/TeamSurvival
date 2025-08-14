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
    public float currentHP; //����ü��
    public float maxHP; //�ִ�ü��
    public float moveSpeed; //�̵��ӵ�

    public event Action OnChangeStatuses;

    public void TakePhysicalDamage(int damage)
    {
        
    }
}
