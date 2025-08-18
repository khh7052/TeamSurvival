using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSystem : MonoBehaviour
{
    [Header("Current")]
    [SerializeField] private ItemData currentItem;

    public WeaponType EquippedWeaponType { get; private set; } = WeaponType.None;
    public ToolType EquippedToolType { get; private set; } = ToolType.None;

    [Header("Raycast")]
    [SerializeField] private LayerMask hitMask;

    private float nextUseTime = 0f;

    public Transform handSlot;
    private GameObject viewInst;

    public bool debugLog = true; //테스트용 디버그 온오프 가능

    public void Equip(ItemData data)
    {
        currentItem = data;

        if (currentItem != null && currentItem.isWeapon)
            EquippedWeaponType = currentItem.weaponType;
        else
            EquippedWeaponType = WeaponType.None;

        if (currentItem != null && currentItem.isTool)
            EquippedToolType = currentItem.toolType;
        else
            EquippedToolType = ToolType.None;

        if (viewInst != null) Destroy(viewInst);
        if (currentItem != null && currentItem.Prefab != null && handSlot != null)
            viewInst = Instantiate(currentItem.Prefab, handSlot, false);
    }

    public void UnEquip()
    {
        currentItem = null;
        EquippedWeaponType = WeaponType.None;
        EquippedToolType = ToolType.None;

        if (viewInst != null) Destroy(viewInst);
    }

    public void Attack()
    {
        if (currentItem == null)
        {
            if (debugLog) Debug.Log("[Equip] Attack blocked: no item", this);
            return;

        }

        if (Time.time < nextUseTime) return;

        if (debugLog) Debug.Log("[Equip] Attack with " + currentItem.name, this);

        if (currentItem.isWeapon)
        {
            UseWeapon();
            float delay = currentItem.weaponAttackDelay;
            if (delay < 0.1f) delay = 0.1f;
            nextUseTime = Time.time + delay;
        }
        else if (currentItem.isTool)
        {
            UseTool();
            nextUseTime = Time.time + 0.2f;
        }
    }

    private void UseWeapon()
    {
        float dist = currentItem.weaponAttackDistance;
        RaycastHit hit; 
        if (Ray(out hit, dist))
        {
            IDamageable dmg = hit.collider.GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakePhysicalDamage(currentItem.weaponDamage);
            }
        }
    }

    private void UseTool()
    {
        float dist = currentItem.toolDistance;
        RaycastHit hit;

        if (Ray(out hit, dist))
        {
            ResourceNode node = hit.collider.GetComponentInParent<ResourceNode>();
            if (node != null)
            {
                if (debugLog) Debug.Log("[Equip] Hit resource: " + node.resourceName, this);

                if (currentItem.toolType != ToolType.None)
                    node.GatherWithTool(currentItem.toolType, currentItem.toolGatherPower);
                else
                    node.Gather(currentItem.toolGatherPower);
            }
        }
    }

    private bool Ray(out RaycastHit hit, float distance)
    {
        Vector3 origin, dir;

        if (Camera.main != null)
        {
            origin = Camera.main.transform.position;
            dir = Camera.main.transform.forward;
        }
        else
        {
            origin = transform.position + Vector3.up * 0.8f;
            dir = transform.forward;
        }
        return Physics.Raycast(origin, dir, out hit, distance, hitMask);
    }
}
