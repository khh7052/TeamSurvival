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

    [Header("Stamina")]
    [SerializeField] private EntityModel model;              // stamina�� ���� �ִ� ��
    [SerializeField] private float staminaCostWeapon = 10f;  // ���� �ֵθ� �� �Ҹ�
    [SerializeField] private float staminaCostTool = 10f;     // ���� ��� �� �Ҹ�
    [SerializeField] private bool blockWhenNoStamina = true; // ���¹̳� ���� �� �ൿ ����

    [Header("�Ǽ�")]
    [SerializeField] private bool allowUnarmedAttack = true;
    [SerializeField] private float unarmedDistance = 2.0f;
    [SerializeField] private float unarmedDelay = 0.5f;
    [SerializeField] private float unarmedStaminaCost = 4f;
    [SerializeField] private int unarmedDamage = 1;      // �Ǽ� ������
    [SerializeField] private int unarmedGatherPower = 1; // �Ǽ����� ������ ���ϰ� ĳ������

    [SerializeField] private string[] unarmedAllowedTags = new[] { "Tree" };
    [SerializeField] private string[] unarmedAllowedNameKeywords = new[] { "Tree", "����" };

    public bool debugLog = true; //�׽�Ʈ�� ����� �¿��� ����

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

        float cost = 0f;
        if (currentItem.isWeapon) cost = staminaCostWeapon;
        else if (currentItem.isTool) cost = staminaCostTool;

        if (!TryConsumeStamina(cost))
        {
            if (blockWhenNoStamina)
            {
                if (debugLog) Debug.Log("[Equip] Blocked: not enough stamina", this);
                return;
            }
        }

        if (debugLog) Debug.Log("[Equip] Attack with " + currentItem.name, this);

        if (currentItem.isWeapon)
        {
            UseWeapon();
            float delay = currentItem.weaponAttackDelay;
            if (delay < 0.1f) delay = 0.7f;
            nextUseTime = Time.time + delay;
        }
        else if (currentItem.isTool)
        {
            UseTool();
            nextUseTime = Time.time + 0.5f;
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

    private bool TryConsumeStamina(float cost)
    {
        if (model == null || model.stamina == null) return true;

        if (cost <= 0f) return true;

        float cur = model.stamina.CurValue;

        if (cur < cost)
        {
            if (blockWhenNoStamina)
            {
                if (debugLog) Debug.Log($"[Equip] Stamina need {cost}, have {cur} �� blocked", this);
                return false;
            }
            else
            {
                if (cur > 0f) model.stamina.Subtract(cur);
                if (debugLog) Debug.Log($"[Equip] Stamina partial consume {cur} (needed {cost})", this);
                return true;
            }
        }

        model.stamina.Subtract(cost);
        if (debugLog) Debug.Log($"[Equip] Stamina -{cost} �� {model.stamina.CurValue:0.##}", this);
        return true;
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
