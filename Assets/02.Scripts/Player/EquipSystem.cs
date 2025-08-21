using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSystem : MonoBehaviour
{
    [Header("Current")]
    [SerializeField] private ItemData currentItem;
    [SerializeField] private GameObject currentItemInstance;

    public WeaponType EquippedWeaponType { get; private set; } = WeaponType.None;
    public ToolType EquippedToolType { get; private set; } = ToolType.None;

    [Header("Raycast")]
    [SerializeField] private LayerMask hitMask;

    private float nextUseTime = 0f;

    public Transform handSlot;
    private GameObject viewInst;

    [Header("Stamina")]
    [SerializeField] private EntityModel model;              // stamina를 갖고 있는 모델
    [SerializeField] private float staminaCostWeapon = 10f;  // 무기 휘두를 때 소모
    [SerializeField] private float staminaCostTool = 10f;     // 도구 사용 시 소모
    [SerializeField] private bool blockWhenNoStamina = true; // 스태미나 부족 시 행동 차단

    [Header("맨손")]
    [SerializeField] private bool allowUnarmedAttack = true;
    [SerializeField] private float unarmedDistance = 2.0f;
    [SerializeField] private float unarmedDelay = 0.5f;
    [SerializeField] private float unarmedStaminaCost = 4f;
    [SerializeField] private int unarmedDamage = 1;      // 맨손 데미지
    [SerializeField] private int unarmedGatherPower = 1; // 맨손으로 나무만 약하게 캐지도록

    public Transform EquipTransform;

    public bool debugLog = true; //테스트용 디버그 온오프 가능

    public void Equip(ItemData data)
    {
        Debug.Log($"[Equip] data={data.name}, isTool={data.isTool}, toolType={data.toolType}");

#if UNITY_EDITOR
        Debug.Log($"[Equip] path={UnityEditor.AssetDatabase.GetAssetPath(data)}");
#endif

        currentItem = data;

        if (viewInst != null) Destroy(viewInst);
        if (currentItemInstance != null) Destroy(currentItemInstance);

        currentItemInstance = new GameObject();
        currentItemInstance.transform.SetParent(EquipTransform, false);
        currentItemInstance.transform.localPosition = Vector3.zero;
        currentItemInstance.transform.localRotation = Quaternion.identity;

        EquippedWeaponType = currentItem.isWeapon ? currentItem.weaponType : WeaponType.None;
        EquippedToolType = currentItem.isTool ? currentItem.toolType : ToolType.None;

        viewInst = Instantiate(currentItem.Prefab, handSlot, false);

        int equipLayer = LayerMask.NameToLayer("Equipment");
        foreach (Transform t in viewInst.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = equipLayer;

        foreach (var c in viewInst.GetComponentsInChildren<Collider>(true)) c.enabled = false;
        foreach (var rb in viewInst.GetComponentsInChildren<Rigidbody>(true)) Destroy(rb);
    }

    public void UnEquip()
    {
        currentItem = null;
        Destroy(currentItemInstance);
        currentItemInstance = null;
        EquippedWeaponType = WeaponType.None;
        EquippedToolType = ToolType.None;

        if (viewInst != null) Destroy(viewInst);
    }

    public void Attack()
    {
        if (Time.time < nextUseTime) return;

        bool hasWeapon = currentItem != null && currentItem.isWeapon;
        bool hasTool = currentItem != null && currentItem.isTool;

        float cost = 0f;
        if (hasWeapon) cost = staminaCostWeapon;
        else if (hasTool) cost = staminaCostTool;
        else if (allowUnarmedAttack) cost = unarmedStaminaCost;

        if (!TryConsumeStamina(cost))
        {
            if (blockWhenNoStamina)
            {
                if (debugLog) Debug.Log("[Equip] Blocked: not enough stamina", this);
                return;
            }
        }

        if (debugLog)
        {
            Debug.Log("[Equip] Attack with " + (hasWeapon || hasTool ? currentItem.name : "(Unarmed)"), this);
        }

        if (hasWeapon)
        {
            UseWeapon();
            float delay = Mathf.Max(currentItem.weaponAttackDelay, 0.7f);
            nextUseTime = Time.time + delay;
        }
        else if (hasTool)
        {
            UseTool();
            nextUseTime = Time.time + 0.5f;
        }
        else if (allowUnarmedAttack)
        {
            UseUnarmed();
            nextUseTime = Time.time + Mathf.Max(unarmedDelay, 0.3f);
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
        Debug.Log($"[EquipSystem] UseTool -> tool={currentItem.toolType}, power={currentItem.toolGatherPower}");

        float dist = currentItem.toolDistance;
        RaycastHit hit;

        if (Ray(out hit, dist))
        {
            ResourceNode node = hit.collider.GetComponentInParent<ResourceNode>();
            if (node != null)
            {
                if (debugLog)
                    Debug.Log("[Equip] Attack with " + (currentItem != null ? currentItem.name : "(Unarmed)"), this);

                if (currentItem.toolType != ToolType.None)
                    node.GatherWithTool(currentItem.toolType, currentItem.toolGatherPower);
                else
                    node.Gather(currentItem.toolGatherPower);
            }
        }
    }

    private void UseUnarmed()
    {
        if (debugLog) Debug.Log("[Equip] Unarmed swing", this);

        if (Ray(out var hit, unarmedDistance))
        {
            if (unarmedDamage > 0)
            {
                var dmg = hit.collider.GetComponentInParent<IDamageable>();
                if (dmg != null) dmg.TakePhysicalDamage(unarmedDamage);
            }

            var node = hit.collider.GetComponentInParent<ResourceNode>();
            if (node != null && node.kind == ResourceKind.Tree)
            {
                if (unarmedGatherPower > 0)
                    node.Gather(unarmedGatherPower);
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
                if (debugLog) Debug.Log($"[Equip] Stamina need {cost}, have {cur} → blocked", this);
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
        if (debugLog) Debug.Log($"[Equip] Stamina -{cost} → {model.stamina.CurValue:0.##}", this);
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
