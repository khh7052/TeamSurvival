using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    //현재 문제 / 수정예정
    //Resource끼리 겹쳐서 스폰
    //너무 갑작스럽게 생성 => 시야밖에 있을때 생성 or 아래에서 위로 솟아나게 수정예정
    //Player 위치에서 Spawn될 경우 끼일수 있음

    [Header("Spawn Setting")]
    public int resourceID; //Factor에서 생성할 Data ID => addressable에서 Resource타입만 가져올수 있으면 필요X
    public float spawnInterval = 3f; //생성주기
    public Vector3 spawnAreaMin; //스폰 최소 좌표
    public Vector3 spawnAreaMax; //스폰 최대 좌표
    public int maxSpawn = 10; //맵에 존재할수 있는 최대 자원 수
    private int currentSpawn = 0; //현재 맵에있는 자원

    private bool canSpawn = false;

    private void Start()
    {
        StartCoroutine(WaitForFactorySpawn());
    }

    IEnumerator WaitForFactorySpawn()
    {
        while(!Factory.Instance.IsInitialized || !ObjectPoolingManager.Instance.IsInitialized)  //초기화전까지 대기
        {
            yield return null;
        }

        canSpawn = true;

        while(canSpawn)
        {
            SpawnResource();
            yield return new WaitForSeconds(spawnInterval); //주기동안 반복
        }
    }

    void SpawnResource()
    {
        if (currentSpawn >= maxSpawn) return;

        Vector3 spawnPos = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            ); //생성 좌표 랜덤

        GameObject resource = Factory.Instance.CreateByID<ItemData>(resourceID); //Factory에서 ID기반으로 Prefab 가져옴
        if(resource != null)
        {
            resource.transform.position = spawnPos;
            currentSpawn++;
        }  
    }
}
