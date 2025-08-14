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

    [System.Serializable]
    public class ItemDataConsumable
    {
        public ConsumableType type;
        public float value;
    }

    [CreateAssetMenu(fileName = "Item", menuName = "New Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Info")]
        public string displayName; //ǥ�õ� �̸�
        public string description; //����
        public ItemType type;
        public Sprite icon;
        public GameObject dropPrefab; //���̴� ������

        [Header("Stacking")] 
        public bool canStack; //������ ��ĭ�� �ߺ� ����
        public int maxStackAmount; //�ߺ� ���� ����

        [Header("Consumable")]
        public ItemDataConsumable[] consumables; //�Ҹ�ǰ ȿ��
    }