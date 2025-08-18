using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ScriptableObjectDataBase<T> where T : BaseScriptableObject
{
    private Dictionary<int, T> _data;


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
        Debug.Log($"[ScriptableObjectDatabase] {label} 라벨의 {_data.Count}개의 {typeof(T).Name} 데이터 로드 완료");
    }

    public T GetById(int id)
    {
        _data.TryGetValue(id, out var result);
        return result;
    }
}
