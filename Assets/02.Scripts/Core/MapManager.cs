using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{

    [Header ("Mesh")]
    [SerializeField]
    private MeshFilter groundMesh;

    [Header("�ڿ� ���� ����")]
    [SerializeField]
    private Transform spawnAreasParent;
    [SerializeField]
    private Collider[] spawnAreas;
    [SerializeField]
    private LayerMask groundLayer;

    [Header("���� �ɼ�")]
    [SerializeField]
    private int spawnMaxCount = 1;
    [SerializeField]
    private int spawnMinCount = 5;
    [SerializeField]
    private float spawnHeightOffset = 0.1f;
    [SerializeField]
    private float minDistanceBetweenObjs = 5f;
    [SerializeField]
    private LayerMask GatheringLayer;

    [Header("������")]
    [SerializeField]
    private float respawnMinDelay = 30f;
    [SerializeField]
    private float respawnMaxDelay = 60f;
    private float respawnTime;
    private float lastSpawnT;

    bool isInitialize = false;

    protected override async void Initialize()
    {
        base.Initialize();
        groundMesh = GetComponent<MeshFilter>();
        spawnAreas = spawnAreasParent.GetComponentsInChildren<Collider>();
        foreach (Collider c in spawnAreas)
        {
            c.isTrigger = true;
        }
//        respawnTime = UnityEngine.Random.Range(respawnMinDelay, respawnMaxDelay);
        await WaitForInstantiate();
    }

    async Task WaitForInstantiate()
    {
        while (!GameManager.Instance.IsInitialized)
        {
            await Task.Yield();
        }
        isInitialize = true;
    }

    private List<Vector3> spawnPositions = new List<Vector3>();

    private async void SpawnObjects()
    {
        spawnPositions.Clear();
        respawnTime = UnityEngine.Random.Range(respawnMinDelay, respawnMaxDelay);
        lastSpawnT = Time.time;
        await WaitForInstantiate();
        if (groundMesh == null || spawnAreas == null)
        {
            Debug.LogWarning("Spawner ������ �ùٸ��� �ʽ��ϴ�.");
            return;
        }

        for (int i = 0; i < spawnAreas.Length; i++)
        {
            int spawnCount = UnityEngine.Random.Range(spawnMinCount, spawnMaxCount);
            for (int j = 0; j < spawnCount; j++)
            {
                Vector3 randPos = GetRandomPointInArea(spawnAreas[i].bounds);
                if (Physics.Raycast(randPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f, groundLayer))
                {
                    if (hit.collider.gameObject == groundMesh.gameObject)
                    {
                        // ������ ������Ʈ ����
                        var randData = GatheringManager.Instance.GetRandomObjectData<BaseScriptableObject>();

                        
                        Vector3 spawnPos = hit.point + Vector3.up * spawnHeightOffset;
                        // ���� ��ȯ�Ǵ� ��ü�� �񵿱� �����̶� ���� �Ǳ� ���� ���� ��ġ�� ����� �� ����. ���� ����Ʈ�� �̸� ���� ����
                        if (spawnPositions.Any(pos => Vector3.Distance(pos, spawnPos) < minDistanceBetweenObjs))
                        {
                            Debug.Log("����� ��ġ�� ������� ��ȯ ���");
                            continue;
                        }
                        
                        // �̹� ������ �Ϸ�Ǿ��ų� ���� ���������� ������ ������Ʈ�� �浹 �˻�
                        Collider[] colliders = Physics.OverlapSphere(spawnPos, minDistanceBetweenObjs, GatheringLayer);
                        if (colliders.Length > 0)
                        {
                            // �̹� �����ϸ� ���� �ǳʶ�
                            Debug.Log("������� ��ȯ ����");
                            continue;
                        }

                        // ��ȯ ��ġ ����
                        spawnPositions.Add(spawnPos);

                        // ���� �� �ݹ����� ��ȯ ��ġ���� ����
                        GameObject go = await Factory.Instance.CreateByAssetReferenceAsync(randData, (go) =>
                        {
                            go.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
                        });

                    }
                    else
                    {
                        Debug.Log(hit.collider.gameObject);
                    }
                }
            }
        }
        Debug.Log($"Time : {Time.time} RespawnDelay : {respawnTime}");
    }

    private void Update()
    {
        if(isInitialize)
            if(Time.time -  lastSpawnT >= respawnTime)
            {
                Debug.Log($"Time : {Time.time - lastSpawnT} RespawnDelay : {respawnTime}");
                SpawnObjects();
            }
    }

    private Vector3 GetRandomPointInArea(Bounds bounds)
    {
        float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.max.y + 1f; // Raycast ���� ����
        return new Vector3(x, y, z);
    }
}
