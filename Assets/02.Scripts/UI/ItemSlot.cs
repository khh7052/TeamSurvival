using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;

    public UIInventory inventory;
    public Button button;
    public Image icon;
    public TextMeshProUGUI quatityText;
    private Outline outline;

    public int index;
    public bool equipped;   //����� ���·θ� ��� (���� ���̶���Ʈ�� UIInventory���� Outline ���� ��)
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

        Clear();
    }

    private void OnEnable()
    {
        if (outline != null) outline.enabled = equipped; // ��� ���̶���Ʈ�� �ݿ�
    }

    public void Set()
    {
        if (icon != null)
        {
            icon.sprite = item != null ? item.icon : null;
            icon.gameObject.SetActive(item != null);
        }

        if (quatityText != null)
            quatityText.text = (item != null && quantity > 1) ? quantity.ToString() : string.Empty;

        if (outline != null)
            outline.enabled = equipped; // ���� ���̶���Ʈ�� UIInventory�� ���� ����
    }

    public void Clear()
    {
        item = null;
        quantity = 0;

        if (icon != null)
        {
            icon.sprite = null;
            icon.gameObject.SetActive(false);
        }
        if (quatityText != null) quatityText.text = string.Empty;
        if (outline != null) outline.enabled = false;
    }

    public void OnClickButton()
    {
        inventory.SelectItem(index);
    }
}
