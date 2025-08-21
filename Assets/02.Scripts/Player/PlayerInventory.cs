using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private EntityModel condition;

    public List<ItemSlot> slots = new();
    public Action OnChangeData;

    public System.Action OnInventoryChanged;

    Dictionary<int, int> itemQuantityCache = new();

    [SerializeField] private SoundData equipSFX;
    [SerializeField] private SoundData eatSFX;

    private void Awake()
    {
        GetComponent<Player>().addItem += Add;
        if (condition == null) condition =  GetComponent<EntityModel>();
    }

    private void ItemCacheInInventory(int id, int Quantity)
    {
        if (itemQuantityCache.ContainsKey(id))
        {
            // 있으면 증감
            itemQuantityCache[id] += Quantity;
            // 0 이하일 경우 삭제
            if (itemQuantityCache[id] <= 0)
            {
                itemQuantityCache.Remove(id);
            }
        }
        else
        {
            // 없으면 해당 값 부여
            if(Quantity > 0)
            {
                itemQuantityCache[id] = Quantity;
            }
        }

    }

    // 아이템 추가
    public void Add() {

        ItemData data = GameManager.player.itemData;
        if (data == null) return;

        AudioManager.Instance.PlaySFX(equipSFX, transform.position);

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
        if (data?.ID == null || GameManager.player.dropPosition == null) return;
        AssetDataLoader.Instance.InstantiateByID(data.ID, (go) =>
        {
            go.transform.position = GameManager.player.dropPosition.position;
            go.transform.rotation = Quaternion.Euler(Vector3.one * UnityEngine.Random.value * 360f);
        });
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

        AudioManager.Instance.PlaySFX(eatSFX, transform.position);

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

        ItemCacheInInventory(slots[index].item.ID, -1);

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
            case ConsumableType.Stamina: condition.stamina.Add(c.value); break;
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

    public async void ItemCreate(CompositionRecipeData data)
    {
        var itemData = await AssetDataLoader.Instance.GetDataByID<ItemData>(data.ID);
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

    public void RemoveItem(int[] itemIds, int[] counts)
    {
        if (itemIds.Length != counts.Length) return;

        for (int i = 0; i < itemIds.Length; i++)
        {
            int remaining = counts[i]; // 남은 삭제 수량

            ItemCacheInInventory(itemIds[i], -counts[i]);

            if (remaining > 0)
            {
                for (int j = 0; j < slots.Count; j++)
                {
                    var slot = slots[j];
                    if (slot.item != null && slot.item.ID == itemIds[i])
                    {
                        int removeAmount = Math.Min(slot.Quantity, remaining);
                        slot.Quantity -= removeAmount;
                        remaining -= removeAmount;

                        if (slot.Quantity <= 0)
                            slot.item = null;

                        if (remaining <= 0) break; // 다 삭제했으면 종료
                    }
                }
            }
        }

        OnChangeData?.Invoke();
    }
}

[Serializable]
public class ItemSlot
{
    public ItemData item;
    public int Quantity;
}