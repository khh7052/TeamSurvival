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
    [Header("체력관련")]
    public float currentHP; //현재체력
    public float maxHP; //최대체력

    [Header("이동관련")]
    public float moveSpeed; //이동속도
    public float jumpPower; //점프력

    public event Action OnChangeStatuses;

    public void TakePhysicalDamage(int damage)
    {
        
    }
}
