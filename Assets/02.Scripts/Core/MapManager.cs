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

    [Header("자원 스폰 영역")]
    [SerializeField]
    private Transform spawnAreasParent;
    [SerializeField]
    private Collider[] spawnAreas;
    [SerializeField]
    private LayerMask groundLayer;

    [Header("스폰 옵션")]
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

    [Header("리스폰")]
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
        // 자식 오브젝트들의 MeshFilter와 MeshRenderer 정보를 담을 리스트
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        // Map 오브젝트의 모든 자식들을 순회하며 MeshFilter를 가져옵니다.
        foreach (Transform child in transform)
        {
            MeshFilter childMeshFilter = child.GetComponent<MeshFilter>();
            if (childMeshFilter != null)
            {
                meshFilters.Add(childMeshFilter);

                // 베이킹을 위해 CombineInstance를 생성하고 추가
                CombineInstance combine = new CombineInstance();
                combine.mesh = childMeshFilter.sharedMesh;
                combine.transform = childMeshFilter.transform.localToWorldMatrix;
                combineInstances.Add(combine);

                // 기존 자식 오브젝트의 렌더러는 비활성화하여 렌더링되지 않도록 합니다.
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    childRenderer.enabled = false;
                }
            }
        }

        // 새로운 MeshFilter 컴포넌트를 추가하거나 가져옵니다.
        MeshFilter mapMeshFilter = GetComponent<MeshFilter>();
        if (mapMeshFilter == null)
        {
            mapMeshFilter = gameObject.AddComponent<MeshFilter>();
        }

        // 새로운 MeshRenderer 컴포넌트를 추가하거나 가져옵니다.
        MeshRenderer mapMeshRenderer = GetComponent<MeshRenderer>();
        if (mapMeshRenderer == null)
        {
            mapMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        // 새로운 Mesh를 생성합니다.
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineInstances.ToArray());

        // 생성된 Mesh를 Map 오브젝트의 MeshFilter에 할당합니다.
        mapMeshFilter.mesh = combinedMesh;

        // 모든 MeshMapTile이 동일한 재질(Material)을 사용한다고 가정하고
        // 첫 번째 자식의 재질을 가져와 새로운 렌더러에 할당합니다.
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
            Debug.LogWarning("Spawner 설정이 올바르지 않습니다.");
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
                        // 랜덤한 오브젝트 선택
                        var randData = AssetDataLoader.Instance.GetRandomAddress(DataType.Gathering);

                        // 생성 위치가 충돌 지점이라 아래로 넣어 울퉁붕퉁한 면 위에서 떠있는 느낌을 줄이기 위함
                        Vector3 spawnPos = hit.point + Vector3.down * spawnHeightOffset;
                        // 현재 소환되는 객체가 비동기 생성이라 생성 되기 전에 다음 위치를 계산할 수 있음. 따라서 리스트로 이를 먼저 관리
                        if (spawnPositions.Any(pos => Vector3.Distance(pos, spawnPos) < minDistanceBetweenObjs))
                        {
//                            Debug.Log("예약된 위치에 가까워서 소환 취소");
                            continue;
                        }
                        
                        // 이미 생성이 완료되었거나 이전 시퀀스에서 생성된 오브젝트도 충돌 검사
                        Collider[] colliders = Physics.OverlapSphere(spawnPos, minDistanceBetweenObjs, GatheringLayer);
                        if (colliders.Length > 0)
                        {
                            // 이미 존재하면 스폰 건너뜀
//                            Debug.Log("가까워서 소환 못함");
                            continue;
                        }

                        // 소환 위치 저장
                        spawnPositions.Add(spawnPos);

                        // 생성 후 콜백으로 소환 위치에서 제거
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
        float y = bounds.max.y + 1f; // Raycast 시작 높이
        return new Vector3(x, y, z);
    }
}
