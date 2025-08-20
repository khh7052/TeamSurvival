using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Resource, //자원
    Equipable, //장착
    Consumable, //소모품
}

public enum ConsumableType
{
    Hunger, //배고픔
    Thirst, //갈증
    Health, //체력
    Stamina, //내구력
}

public enum WeaponType { None, Sword, Bow}
public enum ToolType { None, Axe, Pickaxe}

[System.Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : BaseScriptableObject
{
    [Header("Info")]
    public string description; //설명
    public ItemType type;
    public GameObject dropPrefab; //보이는 프리팹

    [Header("Stacking")] 
    public bool canStack; //아이템 한칸에 중복 여부
    public int maxStackAmount; //중복 개수 제한

    [Header("Consumable")]
    public ItemDataConsumable[] consumables; //소모품 효과

    [Header("Equip Usage")]
    public bool isWeapon;   //무기 용도
    public bool isTool;     //도구 용도

    [Header("Kinds")]
    public WeaponType weaponType = WeaponType.None;
    public ToolType toolType = ToolType.None;

    [Header("Weapon")]
    public int weaponDamage = 10;           //무기 공격력
    public float weaponAttackDelay = 1f;    //무기 공격 딜레이
    public float weaponAttackDistance = 1f; //무기 사거리

    [Header("Tool")]
    public float toolGatherPower = 5f;      //자원 채취 힘
    public float toolDistance = 1f;         //도구 사거리
}