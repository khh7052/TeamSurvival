using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : MonoBehaviour, IHarvestable
{
    [Header("�⺻")]
    public string resourceName = "Resource";
    public float maxDurability = 40f;
    public ToolType preferredTool = ToolType.Axe; // ����=Axe, ��=Pickaxe
    public bool allowBareHands = false;           // �Ǽ� ��� ����

    [Header("ȿ�� ���")]
    public float correctToolMultiplier = 1.5f;    // �´� ����
    public float wrongToolMultiplier = 0.4f;    // Ʋ�� ����
    public float bareHandsMultiplier = 0.25f;   // �Ǽ� ��� ��

    [Header("���")]
    public ItemData yieldItem;       // ItemType.Resource ����, dropPrefab ���� ����
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
        Destroy(gameObject); // ������ ���ϸ� SetActive(false)+Ÿ�̸ӷ� �ٲ㵵 ��
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
