using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition;

    [Header("Selected Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private int curEquipIndex;

    //�߰�: CharacterManager ��� ���� ����
    [SerializeField] private Player player;
    private PlayerController controller;
    private PlayerCondition condition;

    private bool slotsReady = false;

    private void InitSlotsOnce()
    {
        if (slotsReady) return;
        if (slotPanel == null) { Debug.LogError("[UIInventory] slotPanel ���� �ʿ�"); return; }

        int count = slotPanel.childCount;
        slots = new ItemSlot[count];

        for (int i = 0; i < count; i++)
        {
            var child = slotPanel.GetChild(i);
            var s = child.GetComponent<ItemSlot>();
            if (s == null)
            {
                Debug.LogError($"[UIInventory] {child.name}�� ItemSlot�� �����ϴ�.");
                continue;
            }
            s.index = i;
            s.inventory = this;
            // s.Clear();  // �ʿ��ϸ� Ǯ��
            slots[i] = s;
        }

        slotsReady = true;
    }
    private ItemSlot[] Slots => slotPanel ? slotPanel.GetComponentsInChildren<ItemSlot>(true) : System.Array.Empty<ItemSlot>();

    private void Awake()
    {
        // �ڵ� �輱 (������ ã�Ƽ� �ֱ�)
        if (player == null) player = FindFirstObjectByType<Player>();
        if (inventoryWindow == null) inventoryWindow = transform.Find("InventoryWindow")?.gameObject;
        if (slotPanel == null) slotPanel = transform.Find("SlotPanel");
        if (dropPosition == null && player != null) dropPosition = player.dropPosition;
        if (condition == null && player != null) condition = player.GetComponent<PlayerCondition>(); // ������ null ���� ����
    }

    private void Start()
    {
        // �� ���⼭ �ΰ��� Ȯ����
        if (player == null) { Debug.LogError("[UIInventory] Player ����"); enabled = false; return; }
        if (inventoryWindow == null) { Debug.LogError("[UIInventory] inventoryWindow ������"); enabled = false; return; }
        if (slotPanel == null) { Debug.LogError("[UIInventory] slotPanel ������"); enabled = false; return; }

        controller = player.controller;
        if (controller != null) controller.inventory += Toggle;
        player.addItem += AddItem;

        inventoryWindow.SetActive(false);

        // ���Ե� �ε���/������ ���� + �ʱ�ȭ
        var arr = Slots;
        if (arr.Length == 0) Debug.LogWarning("[UIInventory] slotPanel �Ʒ��� ItemSlot�� �����ϴ�.");
        for (int i = 0; i < arr.Length; i++)
        {
            var s = arr[i];
            if (s == null) continue;
            s.index = i;
            s.inventory = this;
            s.Clear();
        }

        ClearSelectedItemWindow();
    }

    private void OnDestroy()
    {
        if (controller != null) controller.inventory -= Toggle;
        if (player != null) player.addItem -= AddItem;
    }

    void ClearSelectedItemWindow()
    {
        selectedItem = null;

        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void Toggle()
    {
        inventoryWindow.SetActive(!IsOpen());
    }

    public bool IsOpen() => inventoryWindow.activeInHierarchy;

    // ����: CharacterManager ��� player ���� ���
    public void AddItem()
    {
        InitSlotsOnce();

        ItemData data = player.itemData;
        if (data == null) return;

        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            player.itemData = null;
            return;
        }

        // ���� ������ �ٽ� ������
        ThrowItem(data);
        player.itemData = null;
    }

    public void UpdateUI()
    {
        var arr = Slots;
        for (int i = 0; i < arr.Length; i++)
        {
            var s = arr[i];
            if (s == null) continue;

            if (s.item != null) s.Set();
            else s.Clear();

            // ����/��� ���̶���Ʈ(Outline)
            var ol = s.GetComponent<Outline>();
            if (ol) ol.enabled = s.equipped || (i == selectedItemIndex && s.item != null);
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        foreach (var s in Slots)
            if (s != null && s.item == data && s.quantity < data.maxStackAmount)
                return s;
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        foreach (var s in Slots)
            if (s != null && s.item == null)
                return s;
        return null;
    }

    public void ThrowItem(ItemData data)
    {
        if (data?.dropPrefab == null || dropPosition == null) return;
        Instantiate(
            data.dropPrefab,
            dropPosition.position,
            Quaternion.Euler(Vector3.one * Random.value * 360f)
        );
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        string title = !string.IsNullOrEmpty(selectedItem.item.name) ? selectedItem.item.name : "Item";
        selectedItemName.text = title;
        selectedItemDescription.text = selectedItem.item.description;

        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        var consumables = selectedItem.item.consumables;
        if (consumables != null)
        {
            for (int i = 0; i < consumables.Length; i++)
            {
                selectedItemStatName.text += consumables[i].type.ToString() + "\n";
                selectedItemStatValue.text += consumables[i].value.ToString() + "\n";
            }
        }

        // ��� ��ư�� '�����' ����(equipped)���� �����ϵ��� ����
        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !slots[index].equipped);
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(true);

        UpdateUI();
    }

    public void OnUseButton()
    {
        if (selectedItem?.item == null) return;
        if (selectedItem.item.type != ItemType.Consumable) return;

        var consumables = selectedItem.item.consumables;
        if (consumables != null)
        {
            for (int i = 0; i < consumables.Length; i++)
                ApplyConsumable(consumables[i]);
        }

        RemoveSelctedItem();
    }

    private void ApplyConsumable(ItemDataConsumable c)
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

    public void OnDropButton()
    {
        if (selectedItem?.item == null) return;
        ThrowItem(selectedItem.item);
        RemoveSelctedItem();
    }

    void RemoveSelctedItem()
    {
        if (selectedItem == null) return;

        selectedItem.quantity--;

        if (selectedItem.quantity <= 0)
        {
            if (slots[selectedItemIndex].equipped)
                UnEquip(selectedItemIndex);

            selectedItem.item = null;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    // �ּ� ����(��� �ý����� ���߿� Ȯ��)
    public void UnEquip(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        slots[index].equipped = false;
    }

    public bool HasItem(ItemData item, int quantity)
    {
        return false;
    }

    private void UpdateSelectionHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var ol = slots[i].GetComponent<Outline>();
            if (ol != null)
                ol.enabled = ol.enabled || (i == selectedItemIndex && slots[i].item != null);
            // '�����'�̸� ���õ� �����̸�(���� ���̶���Ʈ) ������ ��
        }
    }
}
