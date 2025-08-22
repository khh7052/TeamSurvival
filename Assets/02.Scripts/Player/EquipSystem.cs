using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSystem : MonoBehaviour
{
    [Header("Current")]
    [SerializeField] public ItemData currentItem;
    [SerializeField] private GameObject currentItemInstance;

    public WeaponType EquippedWeaponType { get; private set; } = WeaponType.None;
    public ToolType EquippedToolType { get; private set; } = ToolType.None;

    [Header("Raycast")]
    [SerializeField] private LayerMask hitMask;

    private float nextUseTime = 0f;

    [Header("Slots")]
    public Transform handSlot;
    public Transform EquipTransform;

    private GameObject viewInst;

    [SerializeField] private bool spawnThirdPersonModel = true;
    private GameObject thirdPersonInst;

    private ToolAttackSwing viewSwing;      //1인칭
    private ToolAttackSwing thirdSwing;     //3인칭

    private float currentToolDelay = 0.5f;

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


    public bool debugLog = true; //테스트용 디버그 온오프 가능

    void SetLayerRecursively(GameObject go, int layer)
    {
        foreach (var t in go.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = layer;
    }

    void DisablePhysics(GameObject go)
    {
        foreach (var c in go.GetComponentsInChildren<Collider>(true)) c.enabled = false;
        foreach (var rb in go.GetComponentsInChildren<Rigidbody>(true)) Destroy(rb);
    }

    //1인칭 무기
    void GetFpSwingProfile(ToolType t, out Vector3 euler, out Vector3 offset, out float inT, out float outT, out float delay)
    {
        switch (t)
        {
            case ToolType.Axe:      // 도끼
                euler = new Vector3(-28f, 10f, 0f);
                offset = new Vector3(0.010f, -0.010f, 0.025f);
                inT = 0.09f; outT = 0.12f; delay = 0.60f;
                break;
            case ToolType.Pickaxe:  // 곡괭이
                euler = new Vector3(-38f, -6f, 0f);
                offset = new Vector3(0.000f, -0.015f, 0.020f);
                inT = 0.10f; outT = 0.14f; delay = 0.70f;
                break;
            case ToolType.Hammer:   // 망치
                euler = new Vector3(-32f, 6f, 0f);
                offset = new Vector3(0.008f, -0.012f, 0.020f);
                inT = 0.085f; outT = 0.12f; delay = 0.65f;
                break;
            default:
                euler = new Vector3(-18f, 10f, 0f);
                offset = new Vector3(0.010f, -0.010f, 0.020f);
                inT = 0.08f; outT = 0.12f; delay = 0.50f;
                break;
        }
    }

    //3인칭 도구
    void GetTpSwingProfile(ToolType t, out Vector3 euler, out Vector3 offset, out float inT, out float outT)
    {
        switch (t)
        {
            case ToolType.Axe:
                euler = new Vector3(-16, 8, 0); offset = Vector3.zero; inT = 0.08f; outT = 0.12f; break;
            case ToolType.Pickaxe:
                euler = new Vector3(-20, -6, 0); offset = Vector3.zero; inT = 0.09f; outT = 0.13f; break;
            case ToolType.Hammer:
                euler = new Vector3(-14, 5, 0); offset = Vector3.zero; inT = 0.08f; outT = 0.12f; break;
            default:
                euler = new Vector3(-12, 6, 0); offset = Vector3.zero; inT = 0.08f; outT = 0.12f; break;
        }
    }

    //1인칭 무기
    void GetWeaponFpSwingProfile(WeaponType w, out Vector3 euler, out Vector3 offset, out float inT, out float outT)
    {
        if (w == WeaponType.Sword)
        {
            euler = new Vector3(-22f, 16f, 0f);
            offset = new Vector3(0.012f, -0.006f, 0.020f);
            inT = 0.07f; outT = 0.11f;
        }
        else
        {
            euler = new Vector3(-18f, 10f, 0f);
            offset = new Vector3(0.010f, -0.010f, 0.020f);
            inT = 0.08f; outT = 0.12f;
        }
    }

    //3인칭 무기
    void GetWeaponTpSwingProfile(WeaponType w, out Vector3 euler, out Vector3 offset, out float inT, out float outT)
    {
        if (w == WeaponType.Sword)
        {
            euler = new Vector3(-14f, 10f, 0f);
            offset = Vector3.zero;
            inT = 0.08f; outT = 0.12f;
        }
        else
        {
            euler = new Vector3(-10f, 6f, 0f);
            offset = Vector3.zero;
            inT = 0.08f; outT = 0.12f;
        }
    }

    public void Equip(ItemData data)
    {
        currentItem = data;

        if (viewInst != null) Destroy(viewInst);
        if (thirdPersonInst != null) Destroy(thirdPersonInst);
        if (currentItemInstance != null) Destroy(currentItemInstance);

        currentItemInstance = new GameObject();
        currentItemInstance.transform.SetParent(EquipTransform, false);
        currentItemInstance.transform.localPosition = Vector3.zero;
        currentItemInstance.transform.localRotation = Quaternion.identity;
        currentItemInstance.transform.localScale = Vector3.one;

        EquippedWeaponType = currentItem.isWeapon ? currentItem.weaponType : WeaponType.None;
        EquippedToolType = currentItem.isTool ? currentItem.toolType : ToolType.None;

        viewInst = Instantiate(currentItem.Prefab, handSlot, false);

        int equipLayer = LayerMask.NameToLayer("Equipment");
        if (equipLayer == -1) equipLayer = LayerMask.NameToLayer("Equip");
        SetLayerRecursively(viewInst, equipLayer);
        DisablePhysics(viewInst);

        //1인칭 스윙
        viewSwing = viewInst.AddComponent<ToolAttackSwing>();

        if (currentItem.isTool)
        {
            GetFpSwingProfile(currentItem.toolType, out var e1, out var o1, out var in1, out var out1, out var d1);
            viewSwing.SetProfile(e1, o1, in1, out1);
            currentToolDelay = d1;
        }
        else if (currentItem.isWeapon)
        {
            GetWeaponFpSwingProfile(currentItem.weaponType, out var eW, out var oW, out var inW, out var outW);
            viewSwing.SetProfile(eW, oW, inW, outW);
            // 무기 딜레이는 기존 weaponAttackDelay 사용
        }
        else
        {
            viewSwing.SetProfile(new Vector3(-18, 10, 0), new Vector3(0.01f, -0.01f, 0.02f), 0.08f, 0.12f);
            currentToolDelay = 0.5f;
        }

        //3인칭
        if (spawnThirdPersonModel)
        {
            thirdPersonInst = Instantiate(currentItem.Prefab, currentItemInstance.transform, false);

            int playerLayer = LayerMask.NameToLayer("Player");
            SetLayerRecursively(thirdPersonInst, playerLayer);
            DisablePhysics(thirdPersonInst);

            // 3인칭 스윙
            thirdSwing = thirdPersonInst.AddComponent<ToolAttackSwing>();
            if (currentItem.isTool)
            {
                GetTpSwingProfile(currentItem.toolType, out var e2, out var o2, out var in2, out var out2);
                thirdSwing.SetProfile(e2, o2, in2, out2);
            }
            else if (currentItem.isWeapon)
            {
                GetWeaponTpSwingProfile(currentItem.weaponType, out var eW2, out var oW2, out var inW2, out var outW2);
                thirdSwing.SetProfile(eW2, oW2, inW2, outW2);
            }
            else
            {
                thirdSwing.SetProfile(new Vector3(-12, 6, 0), Vector3.zero, 0.08f, 0.12f);
            }
        }
    }

    public void UnEquip()
    {
        Debug.Log("UnEquip실행");
        currentItem = null;

        if (viewInst != null) { Destroy(viewInst); viewInst = null; }
        if (thirdPersonInst != null) { Destroy(thirdPersonInst); thirdPersonInst = null; }
        if (currentItemInstance != null) { Destroy(currentItemInstance); currentItemInstance = null; }

        EquippedWeaponType = WeaponType.None;
        EquippedToolType = ToolType.None;
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
//                if (debugLog) Debug.Log("[Equip] Blocked: not enough stamina", this);
                return;
            }
        }

        if (debugLog)
        {
//            Debug.Log("[Equip] Attack with " + (hasWeapon || hasTool ? currentItem.name : "(Unarmed)"), this);
        }

        if (hasWeapon)
        {
            UseWeapon();
            nextUseTime = Time.time + Mathf.Max(currentItem.weaponAttackDelay, 0.1f);
        }
        else if (hasTool)
        {
            if (currentItem.toolType == ToolType.Hammer) UseHammer();
            else UseTool();

            nextUseTime = Time.time + Mathf.Max(currentToolDelay, 0.1f);
        }
        else if (allowUnarmedAttack)
        {
            UseUnarmed();
            nextUseTime = Time.time + Mathf.Max(unarmedDelay, 0.3f);
        }

        viewSwing?.Play();
        thirdSwing?.Play(0.8f);
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

    private void UseHammer()
    {
        float dist = currentItem.toolDistance;
        if (Ray(out var hit, dist))
        {
            // 건축물만 맞게
            var build = hit.collider.GetComponentInParent<BuildObj>();
            if (build != null)
            {
                int dmg = Mathf.Max(currentItem.demolitionDamage, 1);
                build.TakePhysicalDamage(dmg);
                if (debugLog) Debug.Log($"[Equip] Hammer hit {build.name}, dmg={dmg}", this);
                return;
            }

            // 망치는 자원 채집 X
            if (debugLog) Debug.Log("[Equip] Hammer hit non-building → ignored", this);
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
//                if (debugLog) Debug.Log($"[Equip] Stamina need {cost}, have {cur} → blocked", this);
                return false;
            }
            else
            {
                if (cur > 0f) model.stamina.Subtract(cur);
//                if (debugLog) Debug.Log($"[Equip] Stamina partial consume {cur} (needed {cost})", this);
                return true;
            }
        }

        model.stamina.Subtract(cost);
//        if (debugLog) Debug.Log($"[Equip] Stamina -{cost} → {model.stamina.CurValue:0.##}", this);
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
