using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    //�ּ� ���?�׳� ��⸸ (���� �ܰ迡�� ����/���� ����)
    [HideInInspector] public List<ItemData> items = new List<ItemData>();

    public System.Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[PlayerInventory] �ߺ� �ν��Ͻ��� �����Ǿ� �� ������Ʈ�� �����մϴ�.");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    // ������ �߰�
    public void Add(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("[Inventory] null �������� �߰��� �� �����ϴ�.");
            return;
        }

        items.Add(item);
        Debug.Log($"[Inventory] ȹ��: {item.name} (�� {items.Count}��)");
        OnInventoryChanged?.Invoke();
    }
}
