using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;

public enum ResourceKind { Tree, Rock, Ore, Other }

public class ResourceNode : MonoBehaviour, IHarvestable
{
    [Header("기본")]
    public string resourceName = "Resource";
    public float maxDurability = 40f;

    [Header("종류(맨손)")]
    public ResourceKind kind = ResourceKind.Other;

    [Header("채집 도구 규칙")]
    public ToolType preferredTool = ToolType.Axe;

    [Header("효율 계수")]
    public float correctToolMultiplier = 1f;    // 맞는 도구
    public float wrongToolMultiplier = 0f;    // 틀린 도구
    public float bareHandsMultiplier = 0.5f;   // 맨손 허용 시

    [Header("재생성")]
    public bool respawn = true;
    public float respawnSeconds = 30f;

    [Header("드랍")]
    public ItemData yieldItem;
    public int dropOnDeplete = 3;

    [Tooltip("기본 드랍 외에 하나 더 떨어뜨리고 싶을 때 사용")]
    public ItemData extraYieldItem;
    public int extraDropOnDeplete = 1;
    [Range(0f, 1f)] public float extraDropChance = 1f;

    [Header("Debug")]
    public bool debugLog = true; //테스트용 디버그 온오프 가능

    private float durability;
    private Collider[] cols;
    private Renderer[] rends;

    private bool IsBareHandsAllowed()
    {
        return kind == ResourceKind.Tree;
    }

    private void Log(string msg)
    {
//        if (debugLog) Debug.Log("[Resource][" + resourceName + "] " + msg, this);
    }

    private void Awake()
    {
        cols = GetComponentsInChildren<Collider>(true);
        rends = GetComponentsInChildren<Renderer>(true);
    }

    private void OnEnable()
    {
        durability = maxDurability;
        SetNodeEnabled(true);
        Log("HP reset = " + durability);
    }

    private void OnDepleted()
    {
        SpawnDrops();

        if (respawn)
        {
            gameObject.SetActive(false);
            //StartCoroutine(RespawnRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator RespawnRoutine()
    {
        SetNodeEnabled(false);

        yield return new WaitForSeconds(respawnSeconds);

        durability = maxDurability;
        SetNodeEnabled(true);
    }

    private void SetNodeEnabled(bool on)
    {
        int i;
        for (i = 0; i < rends.Length; i++) rends[i].enabled = on;
        for (i = 0; i < cols.Length; i++) cols[i].enabled = on;
    }

    public void Gather(float power)
    {
        if (!IsBareHandsAllowed())
        {
            if (debugLog) Log($"Bare hands NOT allowed (kind={kind})");
            return;
        }

        float m = bareHandsMultiplier;
        ApplyDamage(power * m);
        if (debugLog) Log($"Gather (BareHands) power={power} x {m} → hp={durability:0.##}");
    }

    public void GatherWithTool(ToolType tool, float power)
    {
        Debug.Log($"[ResourceNode] GatherWithTool IN tool={tool}, power={power}");

        float m;

        if (tool == ToolType.None)
        {
            if (!IsBareHandsAllowed())
            {
                if (debugLog) Log($"Bare hands NOT allowed (kind={kind})");
                return;
            }
            m = bareHandsMultiplier;
        }
        else
        {
            m = (tool == preferredTool) ? correctToolMultiplier : wrongToolMultiplier;
        }
        ApplyDamage(power * m);
        if (debugLog) Log($"Gather (Tool={tool}) power={power} x {m} → hp={durability:0.##}");
    }

    private void ApplyDamage(float amount)
    {
        if (amount <= 0f) return;

        durability -= amount;
        if (durability <= 0f)
        {
            durability = 0f;
            OnDepleted();
        }
    }

    private void SpawnDrops()
    {
        // 기본 드랍
        if (yieldItem != null)
        {
            for (int i = 0; i < dropOnDeplete; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * 0.3f;
                if (pos.y < transform.position.y) pos.y = transform.position.y + 0.2f;

                AssetDataLoader.Instance.InstantiateByID(yieldItem.ID, go =>
                {
                    if (go == null) return;
                    go.transform.SetPositionAndRotation(pos, transform.rotation);
                });
            }
        }

        // 추가 드랍 (있을 때만)
        if (extraYieldItem != null && extraDropOnDeplete > 0 && extraDropChance > 0f)
        {
            for (int i = 0; i < extraDropOnDeplete; i++)
            {
                if (Random.value > extraDropChance) continue;

                Vector3 pos = transform.position + Random.insideUnitSphere * 0.35f; // 살짝 다른 반경
                if (pos.y < transform.position.y) pos.y = transform.position.y + 0.2f;

                AssetDataLoader.Instance.InstantiateByID(extraYieldItem.ID, go =>
                {
                    if (go == null) return;
                    go.transform.SetPositionAndRotation(pos, transform.rotation);
                });
            }
        }
    }

}
