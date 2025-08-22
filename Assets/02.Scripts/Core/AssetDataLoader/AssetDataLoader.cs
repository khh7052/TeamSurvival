using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetDataLoader : Singleton<AssetDataLoader>, ISingletonResetData
{
    public AssetReferenceData data;
    protected Dictionary<int, string> prefabAddressDict = new();
    protected Dictionary<int, string> dataAddressDict = new();
    Dictionary<string, AsyncOperationHandle> loadedHandles = new();

    protected override void Initialize()
    {
        base.Initialize();
        data = Resources.Load<AssetReferenceData>("Data/ItemAssetList");

        foreach(var d in data.data)
        {
            prefabAddressDict[d.ID] = d.assetAdress;
            dataAddressDict[d.ID] = d.dataAdress;
        }
    }
    public void ResetData()
    {
        foreach(var pref in loadedHandles)
        {
            Addressables.Release(pref);
        }

        loadedHandles.Clear();
    }

    public async void InstantiateByID(int id, Action<GameObject> callback = null)
    {
        await CreateByIDAsync(id, callback);
    }

    public async void InstantiateByAssetReference(string address, Action<GameObject> callback = null)
    {
        await CreateByAddress(address, callback);
    }

    private async Task<GameObject> CreateByIDAsync(int id, Action<GameObject> callback = null)
    {
        // 데이터가 없으면 null 리턴
        if (!prefabAddressDict.ContainsKey(id)) return null;
        if (prefabAddressDict[id] == null) return null;

        var handle = Addressables.LoadAssetAsync<GameObject>(prefabAddressDict[id]);
        await handle.Task;

        GameObject go = ObjectPoolingManager.Instance.Get(handle.Result, default(Vector3), default(Quaternion));
        
        // Addressable 핸들을 저장
        if (!loadedHandles.ContainsKey(prefabAddressDict[id]))
        {
            loadedHandles[prefabAddressDict[id]] = handle;
        }

        if(go != null)
            callback?.Invoke(go);
        return go;
    }

    private async Task<GameObject> CreateByAddress(string address, Action<GameObject> callback = null)
    {
        // 데이터가 없으면 null 리턴
        if(address == null || address.Equals("")) return null;

        // Addressables로 데이터 로딩
        var handle = Addressables.LoadAssetAsync<GameObject>(address);
        await handle.Task;
        GameObject go = ObjectPoolingManager.Instance.Get(handle.Result, default(Vector3), default(Quaternion));

        // Addressable 핸들을 저장
        if (!loadedHandles.ContainsKey(address))
        {
            loadedHandles[address] = handle;
        }

        if (go != null)
            callback?.Invoke(go);
        return go;
    }

    public string GetRandomAddress(DataType type)
    {
        if (data == null || data.data.Count == 0)
            return null;

        // 타입 필터링
        var filtered = data.data
            .Where(d => d.dataType == type && !string.IsNullOrEmpty(d.assetAdress))
            .Select(d => d.assetAdress)
            .ToArray();

        if (filtered.Length == 0)
            return null;

        int index = UnityEngine.Random.Range(0, filtered.Length);
        return filtered[index];
    }

    public string GetPrefabAddressByID(int id)
    {
        if(prefabAddressDict.ContainsKey(id))
            return prefabAddressDict[id];
        return null;
    }

    public async Task<T> GetDataByID<T>(int id) where T : BaseScriptableObject
    {
        if (!dataAddressDict.ContainsKey(id) || dataAddressDict[id].Equals("")) return null;
        var t = Addressables.LoadAssetAsync<T>(dataAddressDict[id]);
        await t.Task;

        if (!loadedHandles.ContainsKey(id.ToString()))
        {
            loadedHandles[id.ToString()] = t;
        }

        T result = t.Result;
        return result;
    }

    public async Task<T[]> GetDatasByType<T>(DataType type) where T : BaseScriptableObject
    {
        if (data == null || data.data.Count == 0)
            return Array.Empty<T>();

        var filtered = data.data
            .Where(d => d.dataType == type && !string.IsNullOrEmpty(d.dataAdress))
            .ToArray();

        if (filtered.Length == 0)
            return Array.Empty<T>();

        List<T> results = new();
        foreach (var d in filtered)
        {
            var handle = Addressables.LoadAssetAsync<T>(d.dataAdress);
            await handle.Task;
            results.Add(handle.Result);

            loadedHandles[d.ID.ToString()] = handle; // ID → Handle 저장
        }

        return results.ToArray();
    }

}

