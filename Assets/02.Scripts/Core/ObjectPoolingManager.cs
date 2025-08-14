using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectPoolingManager : Singleton<ObjectPoolingManager>, IInitializableAsync
{
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new();
    private Dictionary<GameObject, GameObject> instanceToPrefab = new();

    public bool _quitting;
    public bool IsInitialized { get; private set; }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        poolDictionary.Clear();
        instanceToPrefab.Clear();
        
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeAsync();
    }
    public void InitializeAsync()
    {
        poolDictionary = new();
        instanceToPrefab = new();
        IsInitialized = true;
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;
        
        if (!poolDictionary.ContainsKey(prefab))
            poolDictionary[prefab] = new();

        GameObject instanceObject = null;

        while (poolDictionary[prefab].Count > 0)
        {
            instanceObject = poolDictionary[prefab].Dequeue();
            if (instanceObject) break;
        }

        if (instanceObject == null)
        {
            instanceObject = Instantiate(prefab);
            instanceObject.AddComponent<AutoReturnObject>();
        }

        instanceObject.transform.SetPositionAndRotation(position, rotation);
        instanceObject.SetActive(true);

        instanceToPrefab[instanceObject] = prefab;
        return instanceObject;
    }

    public GameObject Get(GameObject prefab, Vector3 position) => Get(prefab, position, Quaternion.identity);

    private void OnApplicationQuit()
    {
        _quitting = true;
    }
    public void Return(GameObject instance)
    {
        if (_quitting)
        {
            return;
        }
        if (instance == null) return;

        if (instanceToPrefab.TryGetValue(instance, out GameObject prefab))
        {
            if (!poolDictionary.ContainsKey(prefab))
                poolDictionary[prefab] = new();

            // 중복 방지
            if (!poolDictionary[prefab].Contains(instance))
            {
                instance.SetActive(false);
                poolDictionary[prefab].Enqueue(instance);
            }
        }
        else
        {
            Destroy(instance);
        }
    }

}
