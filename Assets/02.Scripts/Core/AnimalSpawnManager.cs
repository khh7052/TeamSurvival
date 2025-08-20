using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalSpawnManager : MonoBehaviour
{
    [Header("Spawn Point")]
    [SerializeField] private Transform spawnAreaParents;
    [SerializeField] private Collider[] spawnAreas;
    [SerializeField] private LayerMask groundLayer; //바닥 체크용

    [Header("Spawn Setting")]
    [SerializeField] private int minSpawnCount = 1; //영역당 최소 스폰수
    [SerializeField] private int maxSpawnCount = 2; //영역당 최대 스폰수 
    [SerializeField] private float spawnHeightOffset = 0.2f;
    [SerializeField] private float minDistanceBetweenAnimals = 3f;
    [SerializeField] private LayerMask animalLayer;

    [Header("Respawn Setting")]
    [SerializeField] private float respawnMinDelay = 20f; //최소 리스폰 간격
    [SerializeField] private float respawnMaxDelay = 40f; //최대 리스폰 간격

    [Header("테스트용 Animal Prefab")]
    [SerializeField] private List<GameObject> animalPrefabs; //동물 프리팹

    private float respawnTime;
    private float lastSpawnTime;
    private List<Vector3> spawnPositions = new List<Vector3>();

    async void Start()
    {
        spawnAreas = spawnAreaParents.GetComponentsInChildren<Collider>();
        foreach(var area in spawnAreas)
        {
            area.isTrigger = true;
        }
        await WaitForGameReady();
        SpawnAnimals();
    }

    async System.Threading.Tasks.Task WaitForGameReady()
    {
        while (GameManager.Instance != null && !GameManager.Instance.IsInitialized)
            await System.Threading.Tasks.Task.Yield();
    }

    private void Update()
    {
        if (Time.time - lastSpawnTime >= respawnTime)
            SpawnAnimals();
    }

    void SpawnAnimals()
    {
        if(animalPrefabs == null || animalPrefabs.Count == 0)
        {
            Debug.Log("Prefab is Empty");
            return;
        }

        spawnPositions.Clear();
        respawnTime = Random.Range(respawnMinDelay, respawnMaxDelay);
        lastSpawnTime = Time.time;

        foreach(var area in spawnAreas)
        {
            int spawnCount = Random.Range(minSpawnCount, maxSpawnCount);
            for(int i = 0; i < spawnCount; i++)
            {
                Vector3 randPos = GetRandomPoint(area.bounds);
                if(Physics.Raycast(randPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f, groundLayer))
                {
                    Vector3 spawnPos = hit.point + Vector3.up * spawnHeightOffset;

                    if (spawnPositions.Any(pos => Vector3.Distance(pos, spawnPos) < minDistanceBetweenAnimals))
                        continue;

                    if (Physics.OverlapSphere(spawnPos, minDistanceBetweenAnimals, animalLayer).Length > 0)
                        continue;

                    spawnPositions.Add(spawnPos);

                    GameObject prefab = animalPrefabs[Random.Range(0, animalPrefabs.Count)];
                    Instantiate(prefab, spawnPos, Quaternion.identity);
                }
            }
        }
    }

    private Vector3 GetRandomPoint(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x , bounds.max.x);
        float z = Random.Range(bounds.min.z , bounds.max.z);
        float y = bounds.max.y + 1f;
        return new Vector3(x, y, z);
    }
}
