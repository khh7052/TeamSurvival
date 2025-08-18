using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;

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

    [Header("�����")]
    public bool respawn = true;
    public float respawnSeconds = 30f;

    [Header("���")]
    public ItemData yieldItem;
    public AssetReference dropAssetRef;
    public int dropOnDeplete = 3;

    [Header("Debug")]
    public bool debugLog = true; //�׽�Ʈ�� ����� �¿��� ����

    private float durability;
    private Collider[] cols;
    private Renderer[] rends;

    private void Log(string msg)
    {
        if (debugLog) Debug.Log("[Resource][" + resourceName + "] " + msg, this);
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
            StartCoroutine(RespawnRoutine());
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

    private async void SpawnDrops()
    {
        if (yieldItem == null) return;
        GameObject prefab = yieldItem.dropPrefab;
        if (prefab == null) return;

        for (int i = 0; i < dropOnDeplete; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * 0.3f;
            if (pos.y < transform.position.y) pos.y = transform.position.y + 0.2f;
//            Instantiate(prefab, pos, Quaternion.identity);

            await Factory.Instance.CreateByIDAsync<ItemData>(yieldItem.ID, (go) =>
            {
                go.transform.SetLocalPositionAndRotation(pos, transform.rotation);
            });
        }
    }
}
