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
    [Header("공통 정보")]
    public ItemType itemType;
    public GameObject equipPrefab;

    [Header("무기 스탯")]
    public float damage = 10f;

    [Header("도구 스탯")]
    public float gatheringPower = 5f;
}
