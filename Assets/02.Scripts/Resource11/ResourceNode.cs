using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;

public enum ResourceKind { Tree, Rock, Ore, Other }

public class ResourceNode : MonoBehaviour, IHarvestable
{
    [Header("�⺻")]
    public string resourceName = "Resource";
    public float maxDurability = 40f;

    [Header("����(�Ǽ�)")]
    public ResourceKind kind = ResourceKind.Other;

    [Header("ä�� ���� ��Ģ")]
    public ToolType preferredTool = ToolType.Axe;

    [Header("ȿ�� ���")]
    public float correctToolMultiplier = 1f;    // �´� ����
    public float wrongToolMultiplier = 0f;    // Ʋ�� ����
    public float bareHandsMultiplier = 0.5f;   // �Ǽ� ��� ��

    [Header("�����")]
    public bool respawn = true;
    public float respawnSeconds = 30f;

    [Header("���")]
    public ItemData yieldItem;
    public int dropOnDeplete = 3;

    [Header("Debug")]
    public bool debugLog = true; //�׽�Ʈ�� ����� �¿��� ����

    private float durability;
    private Collider[] cols;
    private Renderer[] rends;

    private bool IsBareHandsAllowed()
    {
        return kind == ResourceKind.Tree;
    }

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
        if (debugLog) Log($"Gather (BareHands) power={power} x {m} �� hp={durability:0.##}");
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
        if (debugLog) Log($"Gather (Tool={tool}) power={power} x {m} �� hp={durability:0.##}");
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
        if (yieldItem == null) return;
        
        //GameObject prefab = yieldItem.dropPrefab;
        //if (prefab == null) return;

        for (int i = 0; i < dropOnDeplete; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * 0.3f;
            if (pos.y < transform.position.y) pos.y = transform.position.y + 0.2f;
            //            Instantiate(prefab, pos, Quaternion.identity);

            AssetDataLoader.Instance.InstantiateByID(yieldItem.ID, (go) =>
            {
                go.transform.SetLocalPositionAndRotation(pos, transform.rotation);
            });
        }
    }
}
