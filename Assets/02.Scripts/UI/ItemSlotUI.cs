using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    public ItemData item;

    public UIInventory inventory;
    public Button button;
    public Image icon;
    public TextMeshProUGUI quatityText;
    private Outline outline;

    public int index;
    public bool equipped;   //장비중 상태로만 사용 (선택 하이라이트는 UIInventory에서 Outline 직접 켬)
    public int quantity;

    private void Awake()
    {
        outline = outline ?? GetComponent<Outline>();
        inventory = inventory ?? GetComponentInParent<UIInventory>();
        button = button ?? GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickButton);
        }

    }

    private void OnEnable()
    {
        if (outline != null) outline.enabled = equipped; // 장비 하이라이트만 반영
    }

    public void Set(ItemSlot targetSlot)
    {
        item = targetSlot.item;
        Debug.Log($"Item : {item.DisplayName}");
        quantity = targetSlot.Quantity;
        if (icon != null)
        {
            Debug.Log($"{item.Icon}");
            icon.sprite = item != null ? item.Icon : null;
            icon.enabled = (item != null && item.Icon != null);
        }

        if (quatityText != null)
            quatityText.text = (item != null && quantity > 1) ? quantity.ToString() : string.Empty;

        if (outline != null)
            outline.enabled = equipped; // 선택 하이라이트는 UIInventory가 따로 켜줌
    }

    public void Clear()
    {
        if (item != null) Debug.Log("아이템이 있는데 Clear 호출");
        item = null;
        quantity = 0;

        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
        if (quatityText != null) quatityText.text = string.Empty;
        if (outline != null) outline.enabled = false;
    }

    public void OnClickButton()
    {
        inventory.SelectItem(index);
    }
}
