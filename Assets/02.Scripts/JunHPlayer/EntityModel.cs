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
    [Header("ü�°���")]
    public float currentHP; //����ü��
    public float maxHP; //�ִ�ü��

    [Header("�̵�����")]
    public float moveSpeed; //�̵��ӵ�
    public float jumpPower; //������

    public event Action OnChangeStatuses;

    public void TakePhysicalDamage(int damage)
    {
        
    }
}
