using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipOnStartTest : MonoBehaviour
{
    [Header("References")]
    public EquipSystem equip;   // ����θ� Start���� GetComponent�� ã��

    [Header("Config")]
    public ItemData startItem;  // ���� ���� ������(Item_Tool_Axe ��)
    public float delaySeconds = 0f; // �ʿ��ϸ� ���� ����

    [Header("Debug")]
    public bool log = true;

    void Start()
    {
        if (equip == null) equip = GetComponent<EquipSystem>();
        if (equip == null)
        {
            if (log) Debug.LogWarning("[EquipOnStart] EquipSystem�� �����ϴ�.");
            return;
        }

        if (startItem == null)
        {
            if (log) Debug.LogWarning("[EquipOnStart] startItem�� ��� �ֽ��ϴ�.");
            return;
        }

        if (delaySeconds > 0f)
            StartCoroutine(EquipAfterDelay());
        else
            EquipNow();
    }

    private IEnumerator EquipAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);
        EquipNow();
    }

    public void EquipNow()
    {
        if (equip != null && startItem != null)
        {
            equip.Equip(startItem);
            if (log) Debug.Log("[EquipOnStart] ����: " + startItem.name);
        }
    }

    // ���߿� �ܺο��� �����۸� �ٲ�ġ�� ���� ���� �޼��� ����
    public void SetStartItem(ItemData newItem)
    {
        startItem = newItem;
    }
}
