using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Spawn Setting")]
    public int resourceID;
    public float spawnInterval = 3f;
    public Vector3 spawnAreaMin;
    public Vector3 spawnAreaMax;

    private bool canSpawn = false;

    private void Start()
    {
        StartCoroutine(WaitForFactorySpawn());
    }

    IEnumerator WaitForFactorySpawn()
    {
        while(!Factory.Instance.IsInitialized || !ObjectPoolingManager.Instance.IsInitialized)
        {
            yield return null;
        }

        canSpawn = true;

        while(canSpawn)
        {
            SpawnResource();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnResource()
    {
        Vector3 spawnPos = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

        GameObject resource = Factory.Instance.CreateByID<ItemData>(resourceID);
        if(resource != null)
        {
            resource.transform.position = spawnPos;
        }  
    }
}
