using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEditor.Progress;

public class ScriptableObjectDataBase<T> where T : BaseScriptableObject
{
    private Dictionary<int, T> _data;

    private T[] cachedValues; // 랜덤하게 찾기 위한 캐싱

    public async Task Initialize(string label)
    {
        if(_data == null)
            _data = new();
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, asset =>
        {
            if (!_data.ContainsKey(asset.ID))
            {
                _data[asset.ID] = asset;
            }
            else
            {
                Debug.LogWarning($"[ScriptableObjectDatabase] 중복된 ID 발견: {asset.ID} ({asset.DisplayName})");
            }

        });

        await handle.Task; // 모든 로드 완료 대기

        cachedValues = new T[_data.Count];
        _data.Values.CopyTo(cachedValues, 0); // 밸류 캐싱
        Debug.Log($"[ScriptableObjectDatabase] {label} 라벨의 {_data.Count}개의 {typeof(T).Name} 데이터 로드 완료");
    }

    public T GetById(int id)
    {
        _data.TryGetValue(id, out var result);
        return result;
    }


    public T GetRandomData()
    {
        int index = UnityEngine.Random.Range(0, cachedValues.Length);
        Debug.Log($"[ScriptableObjectDB] index : {index}, Name : {cachedValues[index]}");
        return cachedValues[index];
    }
}
