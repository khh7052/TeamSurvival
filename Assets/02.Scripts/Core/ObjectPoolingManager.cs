using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectPoolingManager : Singleton<ObjectPoolingManager>, IInitializableAsync
{
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new();
    private Dictionary<GameObject, GameObject> instanceToPrefab = new();
    private bool _quitting;

    private Dictionary<AssetReference, Queue<GameObject>> poolAssetRefDict = new();
    private Dictionary<GameObject, AssetReference> instanceToAssetRef = new();

    public bool IsInitialized { get; private set; }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        poolDictionary.Clear();
        instanceToPrefab.Clear();

        poolAssetRefDict.Clear();
        instanceToAssetRef.Clear();
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

    public async Task<GameObject> GetAsync(AssetReference assetRef, Vector3 position, Quaternion rot)
    {
        if (assetRef == null) return null;
        if (!poolAssetRefDict.ContainsKey(assetRef))
        {
            poolAssetRefDict[assetRef] = new();
        }

        GameObject instanceObject = null;

        while (poolAssetRefDict[assetRef].Count > 0)
        {
            instanceObject = poolAssetRefDict[assetRef].Dequeue();
            if (instanceObject) break;
        }

        if (instanceObject == null)
        {
            var handle = Addressables.InstantiateAsync(assetRef, position, rot);
            instanceObject = await handle.Task;

            instanceObject.AddComponent<AutoReturnObject>();
            instanceToAssetRef[instanceObject] = assetRef;
        }

        instanceObject.transform.SetPositionAndRotation(position, rot);
        instanceObject.SetActive(true);
        return instanceObject;
    }

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
        else if(instanceToAssetRef.TryGetValue(instance, out AssetReference asset))
        {
            if (!poolAssetRefDict.ContainsKey(asset))
            {
                poolAssetRefDict[asset] = new();
            }

            // 중복 방지
            if (!poolAssetRefDict[asset].Contains(instance))
            {
                instance.SetActive(false);
                poolAssetRefDict[asset].Enqueue(instance);
            }
        }
        else
        {
            Destroy(instance);
        }
    }

}
