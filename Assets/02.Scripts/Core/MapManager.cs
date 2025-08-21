using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

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
        dontDestroyOnLoad = false;
        base.Initialize();
        groundMesh = GetComponent<MeshFilter>();
        spawnAreas = spawnAreasParent.GetComponentsInChildren<Collider>();
        foreach (Collider c in spawnAreas)
        {
            c.isTrigger = true;
        }
        CombineMeshes();
        await WaitForInstantiate();
    }

    private void CombineMeshes()
    {
        // �ڽ� ������Ʈ���� MeshFilter�� MeshRenderer ������ ���� ����Ʈ
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        // Map ������Ʈ�� ��� �ڽĵ��� ��ȸ�ϸ� MeshFilter�� �����ɴϴ�.
        foreach (Transform child in transform)
        {
            MeshFilter childMeshFilter = child.GetComponent<MeshFilter>();
            if (childMeshFilter != null)
            {
                meshFilters.Add(childMeshFilter);

                // ����ŷ�� ���� CombineInstance�� �����ϰ� �߰�
                CombineInstance combine = new CombineInstance();
                combine.mesh = childMeshFilter.sharedMesh;
                combine.transform = childMeshFilter.transform.localToWorldMatrix;
                combineInstances.Add(combine);

                // ���� �ڽ� ������Ʈ�� �������� ��Ȱ��ȭ�Ͽ� ���������� �ʵ��� �մϴ�.
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    childRenderer.enabled = false;
                }
            }
        }

        // ���ο� MeshFilter ������Ʈ�� �߰��ϰų� �����ɴϴ�.
        MeshFilter mapMeshFilter = GetComponent<MeshFilter>();
        if (mapMeshFilter == null)
        {
            mapMeshFilter = gameObject.AddComponent<MeshFilter>();
        }

        // ���ο� MeshRenderer ������Ʈ�� �߰��ϰų� �����ɴϴ�.
        MeshRenderer mapMeshRenderer = GetComponent<MeshRenderer>();
        if (mapMeshRenderer == null)
        {
            mapMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        // ���ο� Mesh�� �����մϴ�.
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineInstances.ToArray());

        // ������ Mesh�� Map ������Ʈ�� MeshFilter�� �Ҵ��մϴ�.
        mapMeshFilter.mesh = combinedMesh;

        // ��� MeshMapTile�� ������ ����(Material)�� ����Ѵٰ� �����ϰ�
        // ù ��° �ڽ��� ������ ������ ���ο� �������� �Ҵ��մϴ�.
        if (meshFilters.Count > 0)
        {
            Renderer firstChildRenderer = meshFilters[0].GetComponent<Renderer>();
            if (firstChildRenderer != null)
            {
                mapMeshRenderer.material = firstChildRenderer.sharedMaterial;
            }
        }

        MeshCollider collider = GetComponent<MeshCollider>();
        if(collider == null)
        {
            collider = gameObject.AddComponent<MeshCollider>();
        }
        collider.sharedMesh = combinedMesh;

        foreach(var mesh in meshFilters)
        {
            mesh.gameObject.SetActive(false);
        }
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
                        var randData = AssetDataLoader.Instance.GetRandomAddress(DataType.Gathering);

                        // ���� ��ġ�� �浹 �����̶� �Ʒ��� �־� ���������� �� ������ ���ִ� ������ ���̱� ����
                        Vector3 spawnPos = hit.point + Vector3.down * spawnHeightOffset;
                        // ���� ��ȯ�Ǵ� ��ü�� �񵿱� �����̶� ���� �Ǳ� ���� ���� ��ġ�� ����� �� ����. ���� ����Ʈ�� �̸� ���� ����
                        if (spawnPositions.Any(pos => Vector3.Distance(pos, spawnPos) < minDistanceBetweenObjs))
                        {
//                            Debug.Log("����� ��ġ�� ������� ��ȯ ���");
                            continue;
                        }
                        
                        // �̹� ������ �Ϸ�Ǿ��ų� ���� ���������� ������ ������Ʈ�� �浹 �˻�
                        Collider[] colliders = Physics.OverlapSphere(spawnPos, minDistanceBetweenObjs, GatheringLayer);
                        if (colliders.Length > 0)
                        {
                            // �̹� �����ϸ� ���� �ǳʶ�
//                            Debug.Log("������� ��ȯ ����");
                            continue;
                        }

                        // ��ȯ ��ġ ����
                        spawnPositions.Add(spawnPos);

                        // ���� �� �ݹ����� ��ȯ ��ġ���� ����
                        AssetDataLoader.Instance.InstantiateByAssetReference(randData, (go) =>
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
    }

    private void Update()
    {
        if(isInitialize)
            if(Time.time -  lastSpawnT >= respawnTime)
            {
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
