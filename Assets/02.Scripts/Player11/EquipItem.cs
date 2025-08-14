using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Tool
}

[CreateAssetMenu(menuName = "Game/Equip Item", fileName = "Equip_")]
public class EquipItem : ScriptableObject
{
    [Header("���� ����")]
    public ItemType itemType;
    public GameObject equipPrefab;

    [Header("���� ����")]
    public float damage = 10f;

    [Header("���� ����")]
    public float gatheringPower = 5f;
}
