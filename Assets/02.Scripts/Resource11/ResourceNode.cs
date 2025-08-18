using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : MonoBehaviour, IHarvestable
{
    [Header("기본")]
    public string resourceName = "Resource";
    public float maxDurability = 40f;
    public ToolType preferredTool = ToolType.Axe; // 나무=Axe, 돌=Pickaxe
    public bool allowBareHands = false;           // 맨손 허용 여부

    [Header("효율 계수")]
    public float correctToolMultiplier = 1.5f;    // 맞는 도구
    public float wrongToolMultiplier = 0.4f;    // 틀린 도구
    public float bareHandsMultiplier = 0.25f;   // 맨손 허용 시

    [Header("드랍")]
    public ItemData yieldItem;       // ItemType.Resource 권장, dropPrefab 연결 권장
    public int dropOnDeplete = 3;

    private float durability;

    private void OnEnable()
    {
        durability = maxDurability;
    }

    public void Gather(float power)
    {
        float m = 0f;
        if (allowBareHands) m = bareHandsMultiplier;
        ApplyDamage(power * m);
    }

    public void GatherWithTool(ToolType tool, float power)
    {
        float m;
        if (tool == ToolType.None)
        {
            if (allowBareHands) m = bareHandsMultiplier;
            else m = 0f;
        }
        else
        {
            if (tool == preferredTool) m = correctToolMultiplier;
            else m = wrongToolMultiplier;
        }
        ApplyDamage(power * m);
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

    private void OnDepleted()
    {
        SpawnDrops();
        Destroy(gameObject); // 리스폰 원하면 SetActive(false)+타이머로 바꿔도 됨
    }

    private void SpawnDrops()
    {
        if (yieldItem == null) return;
        GameObject prefab = yieldItem.dropPrefab;
        if (prefab == null) return;

        for (int i = 0; i < dropOnDeplete; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * 0.3f;
            if (pos.y < transform.position.y) pos.y = transform.position.y + 0.2f;
            Instantiate(prefab, pos, Quaternion.identity);
        }
    }
}
