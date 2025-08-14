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
        public string displayName; //표시될 이름
        public string description; //설명
        public ItemType type;
        public Sprite icon;
        public GameObject dropPrefab; //보이는 프리팹

        [Header("Stacking")] 
        public bool canStack; //아이템 한칸에 중복 여부
        public int maxStackAmount; //중복 개수 제한

        [Header("Consumable")]
        public ItemDataConsumable[] consumables; //소모품 효과
    }