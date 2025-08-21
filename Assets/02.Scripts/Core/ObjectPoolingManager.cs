using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectPoolingManager : Singleton<ObjectPoolingManager>
{
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new();
    private Dictionary<GameObject, GameObject> instanceToPrefab = new();
    private bool _quitting;

    private Dictionary<string, Queue<GameObject>> poolAssetRefDict = new();
    private Dictionary<GameObject, string> instanceToAssetRef = new();

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

    public async Task<GameObject> GetAsync(string address, Vector3 position, Quaternion rot)
    {
        if (address == null) return null;
        if (!poolAssetRefDict.ContainsKey(address))
        {
            poolAssetRefDict[address] = new();
        }

        GameObject instanceObject = null;

        while (poolAssetRefDict[address].Count > 0)
        {
            instanceObject = poolAssetRefDict[address].Dequeue();
            if (instanceObject) break;
        }

        if (instanceObject == null)
        {
            var handle = Addressables.InstantiateAsync(address, position, rot);
            instanceObject = await handle.Task;

            instanceObject.AddComponent<AutoReturnObject>();
            instanceToAssetRef[instanceObject] = address;
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
        else if(instanceToAssetRef.TryGetValue(instance, out string adress))
        {
            if (!poolAssetRefDict.ContainsKey(adress))
            {
                poolAssetRefDict[adress] = new();
            }

            // 중복 방지
            if (!poolAssetRefDict[adress].Contains(instance))
            {
                instance.SetActive(false);
                poolAssetRefDict[adress].Enqueue(instance);
            }
        }
        else
        {
            Destroy(instance);
        }
    }

}
