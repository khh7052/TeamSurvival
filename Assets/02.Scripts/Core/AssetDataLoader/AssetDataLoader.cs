using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AssetDataLoader : Singleton<AssetDataLoader>
{
    public AssetReferenceData data;
    protected Dictionary<int, string> prefabAddressDict = new();
    protected Dictionary<int, string> dataAddressDict = new();

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

        GameObject go = await ObjectPoolingManager.Instance.GetAsync(prefabAddressDict[id], Vector3.zero, Quaternion.identity);
        if(go != null)
            callback?.Invoke(go);
        return go;
    }

    private async Task<GameObject> CreateByAddress(string adress, Action<GameObject> callback = null)
    {
        if(adress == null || adress.Equals("")) return null;

        GameObject go = await ObjectPoolingManager.Instance.GetAsync(adress, Vector3.zero, Quaternion.identity);
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

        T result = t.Result;
        return result;
    }

    public async Task<T[]> GetDatasByType<T>(DataType type) where T : BaseScriptableObject
    {
        if (data == null || data.data.Count == 0)
            return Array.Empty<T>();

        var filtered = data.data
            .Where(d => d.dataType == type && !string.IsNullOrEmpty(d.dataAdress))
            .Select(d => d.dataAdress)
            .ToArray();

        if (filtered.Length == 0)
            return Array.Empty<T>();

        // 모든 로드 Task를 동시에 실행
        var tasks = filtered.Select(addr => Addressables.LoadAssetAsync<T>(addr).Task).ToArray();
        var results = await Task.WhenAll(tasks); // 모든 Task가 끝날 때까지 대기

        return results;
    }
}

