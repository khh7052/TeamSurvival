using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private PlayerCondition condition;

    public List<ItemSlot> slots = new();
    public Action OnChangeData;

    public System.Action OnInventoryChanged;

    Dictionary<int, int> itemQuantityCache = new();

    private void Awake()
    {
        GetComponent<Player>().addItem += Add;
    }

    private void ItemCacheInInventory(int id, int Quantity)
    {
        if (itemQuantityCache.ContainsKey(id))
        {
            // 있으면 증가
            itemQuantityCache[id] += Quantity;
        }
        else
        {
            // 없으면 해당 값 부여
            itemQuantityCache[id] = Quantity;
        }

        // 0 이하일 경우 삭제
        if (itemQuantityCache[id] <= 0)
        {
            itemQuantityCache.Remove(id);
        }
    }

    // 아이템 추가
    public void Add() {

        ItemData data = GameManager.player.itemData;
        if (data == null) return;

        if (data.canStack)
        {
            ItemSlot slot = GetItemSlot(data);
            if (slot != null)
            {
                slot.Quantity++;
                GameManager.player.itemData = null;
                ItemCacheInInventory(data.ID, 1);
                OnChangeData?.Invoke();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.Quantity = 1;
            ItemCacheInInventory(data.ID, 1);
            GameManager.player.itemData = null;
            OnChangeData?.Invoke();
            return;
        }

        ThrowItem(data);
        GameManager.player.itemData = null;
    }

    ItemSlot GetItemSlot(ItemData data) 
    {
        foreach (ItemSlot slot in slots)
        {
            if(slot != null && slot.item == data && slot.Quantity < data.maxStackAmount)
            {
                return slot;
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        foreach (var s in slots)
            if (s != null && s.item == null)
                return s;
        return null;
    }

    public void ThrowItem(ItemData data)
    {
        if (data?.dropPrefab == null || GameManager.player.dropPosition == null) return;
        Instantiate(
            data.dropPrefab,
            GameManager.player.dropPosition.position,
            Quaternion.Euler(Vector3.one * UnityEngine.Random.value * 360f)
        );
    }

    public void ThrowItemInInventory(int index)
    {
        ThrowItem(slots[index].item);
        RemoveIndexItem(index);
    }

    public void UseItem(int index)
    {
        if (slots[index].item == null) return;
        if (slots[index].item.type != ItemType.Consumable) return;

        var consumables = slots[index].item.consumables;
        if (consumables != null)
        {
            for (int i = 0; i < consumables.Length; i++)
            {
                ApplyConsume(consumables[i]);
            }
        }

        RemoveIndexItem(index);
    }
    
    private void RemoveIndexItem(int index)
    {
        if(slots[index].item == null) return;
        slots[index].Quantity--;

        if (slots[index].Quantity <= 0)
        {
            slots[index].item = null;
        }
        OnChangeData?.Invoke();
    }

    private void RemoveItem(int itemID)
    {
        foreach(var slot in slots)
        {
            if(slot.item != null && slot.item.ID == itemID)
            {
                slot.Quantity--;

                if(slot.Quantity <= 0)
                {
                    slot.item = null;
                }
                break;
            }
        }
        OnChangeData?.Invoke();
    }

    private void ApplyConsume(ItemDataConsumable c)
    {
        if (condition == null) return;
        switch (c.type)
        {
            case ConsumableType.Health: condition.Heal(c.value); break;
            case ConsumableType.Hunger: condition.Eat(c.value); break;
            case ConsumableType.Thirst: condition.Drink(c.value); break;
            case ConsumableType.Stamina: condition.RecoverStamina(c.value); break;
        }
    }

    public bool IsHasItem(int[] ids, int[] counts)
    {
        if(ids.Length != counts.Length) return false;

        for(int i = 0; i < ids.Length; i++)
        {
            if (itemQuantityCache.ContainsKey(ids[i]))
            {
                if (itemQuantityCache[ids[i]] < counts[i]) return false;    
            }
            else return false;
        }

        return true;
    }

    public void ItemCreate(CompositionRecipeData data)
    {
        var itemData = Factory.Instance.GetDataByID<ItemData>(data.ID);
        GameManager.player.itemData = itemData;

        for (int i = 0; i < data.recipe.Count; i++)
        {
            for (int j = 0; j < data.recipe[i].ItemCount; j++)
            {
                RemoveItem(data.recipe[i].ItemID); // 감소 로직 효율 안좋음.
                ItemCacheInInventory(data.recipe[i].ItemID, -1);
            }
        }

        Add();
        OnChangeData?.Invoke();
    }
}

[Serializable]
public class ItemSlot
{
    public ItemData item;
    public int Quantity;
}