using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Resource, //�ڿ�
    Equipable, //����
    Consumable, //�Ҹ�ǰ
}

public enum ConsumableType
{
    Hunger, //�����
    Thirst, //����
    Health, //ü��
    Stamina, //������
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
    public string description; //����
    public ItemType type;
    public GameObject dropPrefab; //���̴� ������

    [Header("Stacking")] 
    public bool canStack; //������ ��ĭ�� �ߺ� ����
    public int maxStackAmount; //�ߺ� ���� ����

    [Header("Consumable")]
    public ItemDataConsumable[] consumables; //�Ҹ�ǰ ȿ��

    [Header("Equip Usage")]
    public bool isWeapon;   //���� �뵵
    public bool isTool;     //���� �뵵

    [Header("Kinds")]
    public WeaponType weaponType = WeaponType.None;
    public ToolType toolType = ToolType.None;

    [Header("Weapon")]
    public int weaponDamage = 10;           //���� ���ݷ�
    public float weaponAttackDelay = 1f;    //���� ���� ������
    public float weaponAttackDistance = 1f; //���� ��Ÿ�

    [Header("Tool")]
    public float toolGatherPower = 5f;      //�ڿ� ä�� ��
    public float toolDistance = 1f;         //���� ��Ÿ�
}