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

    //추가: CharacterManager 대신 직접 참조
    [SerializeField] private Player player;
    private PlayerController controller;
    private PlayerCondition condition;

    private bool slotsReady = false;

    private void InitSlotsOnce()
    {
        if (slotsReady) return;
        if (slotPanel == null) { Debug.LogError("[UIInventory] slotPanel 지정 필요"); return; }

        int count = slotPanel.childCount;
        slots = new ItemSlot[count];

        for (int i = 0; i < count; i++)
        {
            var child = slotPanel.GetChild(i);
            var s = child.GetComponent<ItemSlot>();
            if (s == null)
            {
                Debug.LogError($"[UIInventory] {child.name}에 ItemSlot이 없습니다.");
                continue;
            }
            s.index = i;
            s.inventory = this;
            // s.Clear();  // 필요하면 풀기
            slots[i] = s;
        }

        slotsReady = true;
    }
    private ItemSlot[] Slots => slotPanel ? slotPanel.GetComponentsInChildren<ItemSlot>(true) : System.Array.Empty<ItemSlot>();

    private void Awake()
    {
        // 자동 배선 (없으면 찾아서 넣기)
        if (player == null) player = FindFirstObjectByType<Player>();
        if (inventoryWindow == null) inventoryWindow = transform.Find("InventoryWindow")?.gameObject;
        if (slotPanel == null) slotPanel = transform.Find("SlotPanel");
        if (dropPosition == null && player != null) dropPosition = player.dropPosition;
        if (condition == null && player != null) condition = player.GetComponent<PlayerCondition>(); // 없으면 null 유지 가능
    }

    private void Start()
    {
        // ★ 여기서 널가드 확실히
        if (player == null) { Debug.LogError("[UIInventory] Player 없음"); enabled = false; return; }
        if (inventoryWindow == null) { Debug.LogError("[UIInventory] inventoryWindow 미지정"); enabled = false; return; }
        if (slotPanel == null) { Debug.LogError("[UIInventory] slotPanel 미지정"); enabled = false; return; }

        controller = player.controller;
        if (controller != null) controller.inventory += Toggle;
        player.addItem += AddItem;

        inventoryWindow.SetActive(false);

        // 슬롯들 인덱스/역참조 세팅 + 초기화
        var arr = Slots;
        if (arr.Length == 0) Debug.LogWarning("[UIInventory] slotPanel 아래에 ItemSlot이 없습니다.");
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

    // 변경: CharacterManager 대신 player 참조 사용
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

        // 공간 없으면 다시 버리기
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

            // 선택/장비 하이라이트(Outline)
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

        // 장비 버튼은 '장비중' 상태(equipped)에만 의존하도록 유지
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

    // 최소 스텁(장비 시스템은 나중에 확장)
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
            // '장비중'이면 선택된 슬롯이면(선택 하이라이트) 강제로 켬
        }
    }
}
