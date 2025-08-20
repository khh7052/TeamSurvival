using JetBrains.Annotations;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIInventory : BaseUI
{
    public ItemSlotUI[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;

    [Header("Selected Item")]
    private ItemSlotUI selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private PlayerInventory inventory;
    private EquipSystem playerEquip;

    private int curEquipIndex = -1;

    private bool slotsReady = false;

    private void InitSlotsOnce()
    {
        if (slotsReady) return;
        if (slotPanel == null) { Debug.LogError("[UIInventory] slotPanel ���� �ʿ�"); return; }

        int count = slotPanel.childCount;
        slots = new ItemSlotUI[count];

        for (int i = 0; i < count; i++)
        {
            var child = slotPanel.GetChild(i);
            var s = child.GetComponent<ItemSlotUI>();
            if (s == null)
            {
                Debug.LogError($"[UIInventory] {child.name}�� ItemSlot�� �����ϴ�.");
                continue;
            }
            s.index = i;
            s.inventory = this;
            slots[i] = s;
        }

        slotsReady = true;
        inventory = GameManager.player.inventory;
        playerEquip = GameManager.player.equip;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        inventory.OnChangeData += UpdateUI;
        Debug.Log("Enable");
        UpdateUI();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        inventory.OnChangeData -= UpdateUI;
    }

    protected override async void Awake()
    {
        await WaitManagerInitial();
        // �ڵ� �輱 (������ ã�Ƽ� �ֱ�)
        if (inventoryWindow == null) inventoryWindow = transform.Find("InventoryWindow")?.gameObject;
        if (slotPanel == null) slotPanel = transform.Find("SlotPanel");
        InitSlotsOnce();
    }

    async Task WaitManagerInitial()
    {
        while (!GameManager.Instance.IsInitialized)
        {
            await Task.Yield();
        }
    }

    private void Start()
    {
        // �� ���⼭ �ΰ��� Ȯ����
        if (inventoryWindow == null) { Debug.LogError("[UIInventory] inventoryWindow ������"); enabled = false; return; }
        if (slotPanel == null) { Debug.LogError("[UIInventory] slotPanel ������"); enabled = false; return; }

        // ���Ե� �ε���/������ ���� + �ʱ�ȭ
        var arr = slots;
        if (arr.Length == 0) Debug.LogWarning("[UIInventory] slotPanel �Ʒ��� ItemSlot�� �����ϴ�.");
        for (int i = 0; i < arr.Length; i++)
        {
            var s = arr[i];
            if (s == null) continue;
            s.index = i;
            s.inventory = this;
        }

        ClearSelectedItemWindow();
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

    public void UpdateUI()
    {
        var arr = slots;
        for (int i = 0; i < arr.Length; i++)
        {
            var s = arr[i];
            if (s == null) continue;

            if (inventory.slots[i].item != null)
            {
                s.Set(inventory.slots[i]);
            }
            else s.Clear();

            // ����/��� ���̶���Ʈ(Outline)
            var ol = s.GetComponent<Outline>();
            if (ol) ol.enabled = s.equipped || (i == selectedItemIndex && s.item != null);
        }
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
        inventory.UseItem(selectedItemIndex);
    }


    public void OnDropButton()
    {
        if (selectedItem?.item == null) return;
        inventory.ThrowItemInInventory(selectedItemIndex);
        if (slots[selectedItemIndex].quantity <= 0)
        {
            ClearSelectedItemWindow();
        }
    }

    public void OnEquipButton()
    {
        Equip(selectedItemIndex);
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }

    public void Equip(int index)
    {
        if(curEquipIndex != -1) UnEquip(curEquipIndex);
        if (index < 0 || index >= slots.Length) return;
        slots[index].equipped = true;
        playerEquip.Equip(slots[index].item);
        curEquipIndex = index; 
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !slots[index].equipped);
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && slots[index].equipped);

    }

    // �ּ� ����(��� �ý����� ���߿� Ȯ��)
    public void UnEquip(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        slots[index].equipped = false;
        playerEquip.UnEquip();
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !slots[index].equipped);
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && slots[index].equipped);

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
