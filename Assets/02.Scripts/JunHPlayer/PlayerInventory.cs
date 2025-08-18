using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    //최소 기능?그냥 담기만 (다음 단계에서 슬롯/스택 적용)
    [HideInInspector] public List<ItemData> items = new List<ItemData>();

    public System.Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[PlayerInventory] 중복 인스턴스가 감지되어 새 컴포넌트를 제거합니다.");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    // 아이템 추가
    public void Add(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("[Inventory] null 아이템은 추가할 수 없습니다.");
            return;
        }

        items.Add(item);
        Debug.Log($"[Inventory] 획득: {item.name} (총 {items.Count}개)");
        OnInventoryChanged?.Invoke();
    }
}
