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

    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //        Use();
    //}

    public void Equip(ItemData data)
    {
        currentItem = data;

        EquippedWeaponType = (currentItem != null && currentItem.isWeapon) ? currentItem.weaponType : WeaponType.None;
        EquippedToolType = (currentItem != null && currentItem.isTool) ? currentItem.toolType : ToolType.None;

        Debug.Log($"[Equip] Weapon = {EquippedWeaponType}, Tool = {EquippedToolType}");
    }

    public void UnEquip()
    {
        currentItem = null;
        EquippedWeaponType = WeaponType.None;
        EquippedToolType = ToolType.None;
    }

    //public void Use()
    //{
    //    if (currentItem == null) return;
    //    if (Time.time < nextUseTime) return;

    //    if (EquippedWeaponType != WeaponType.None)
    //    {
    //        UseWeapon();
    //        nextUseTime = Time.time + Mathf.Max(0.1f, currentItem.weaponAttackDelay);
    //        return;
    //    }

    //    if (EquippedToolType != ToolType.None)
    //    {
    //        UseTool();
    //        nextUseTime = Time.time + 0.2f;
    //        return;
    //    }
    //}

    //private void UseWeapon()
    //{
    //    float dist = currentItem.weaponAttackDistance;

    //    if (Ray(out RaycastHit hit, dist))
    //    {
    //        var monster = hit.collider.GetComponentInParent<Monster>();
    //        if (monster != null)
    //        {
    //            switch (EquippedWeaponType)
    //            {
    //                case WeaponType.Sword:
    //                    monster.TakeDamage(currentItem.weaponDamage);
    //                    break;

    //                case WeaponType.Bow:
    //                    monster.TakeDamage(currentItem.weaponDamage);
    //                    break;
    //                default:
    //                    monster.TakeDamage(currentItem.weaponDamage);
    //                    break;
    //            }
    //            return;
    //        }
    //    }
    //}

    //private void UseTool()
    //{
    //    float dist = currentItem.toolDistance;

    //    if (Ray(out RaycastHit hit, dist))
    //    {
    //        var node = hit.collider.GetComponentInParent<ResourceNode>();
    //        if (node != null)
    //        {
    //            switch (EquippedToolType)
    //            {
    //                case ToolType.Axe:
    //                    node.GatherWithTool(ToolType.Axe, currentItem.toolGatherPower);
    //                    break;

    //                case ToolType.Pickaxe:
    //                    node.GatherWithTool(ToolType.Pickaxe, currentItem.toolGatherPower);
    //                    break;

    //                default:
    //                    node.Gather(currentItem.toolGatherPower);
    //                    break;
    //            }
    //            return;
    //        }
    //    }
    //}

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
