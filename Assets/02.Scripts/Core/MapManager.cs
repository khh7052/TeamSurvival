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
                        var randData = GatheringManager.Instance.GetRandomObjectData<BaseScriptableObject>();

                        
                        Vector3 spawnPos = hit.point + Vector3.up * spawnHeightOffset;
                        // 현재 소환되는 객체가 비동기 생성이라 생성 되기 전에 다음 위치를 계산할 수 있음. 따라서 리스트로 이를 먼저 관리
                        if (spawnPositions.Any(pos => Vector3.Distance(pos, spawnPos) < minDistanceBetweenObjs))
                        {
                            Debug.Log("예약된 위치에 가까워서 소환 취소");
                            continue;
                        }
                        
                        // 이미 생성이 완료되었거나 이전 시퀀스에서 생성된 오브젝트도 충돌 검사
                        Collider[] colliders = Physics.OverlapSphere(spawnPos, minDistanceBetweenObjs, GatheringLayer);
                        if (colliders.Length > 0)
                        {
                            // 이미 존재하면 스폰 건너뜀
                            Debug.Log("가까워서 소환 못함");
                            continue;
                        }

                        // 소환 위치 저장
                        spawnPositions.Add(spawnPos);

                        // 생성 후 콜백으로 소환 위치에서 제거
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
        float y = bounds.max.y + 1f; // Raycast 시작 높이
        return new Vector3(x, y, z);
    }
}
