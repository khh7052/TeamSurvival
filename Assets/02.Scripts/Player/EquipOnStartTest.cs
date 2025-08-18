using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipOnStartTest : MonoBehaviour
{
    [Header("References")]
    public EquipSystem equip;   // 비워두면 Start에서 GetComponent로 찾음

    [Header("Config")]
    public ItemData startItem;  // 시작 장착 아이템(Item_Tool_Axe 등)
    public float delaySeconds = 0f; // 필요하면 지연 장착

    [Header("Debug")]
    public bool log = true;

    void Start()
    {
        if (equip == null) equip = GetComponent<EquipSystem>();
        if (equip == null)
        {
            if (log) Debug.LogWarning("[EquipOnStart] EquipSystem이 없습니다.");
            return;
        }

        if (startItem == null)
        {
            if (log) Debug.LogWarning("[EquipOnStart] startItem이 비어 있습니다.");
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
            if (log) Debug.Log("[EquipOnStart] 장착: " + startItem.name);
        }
    }

    // 나중에 외부에서 아이템만 바꿔치기 쉽게 공개 메서드 제공
    public void SetStartItem(ItemData newItem)
    {
        startItem = newItem;
    }
}
