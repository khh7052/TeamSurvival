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

    private T[] cachedValues; // �����ϰ� ã�� ���� ĳ��

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
                Debug.LogWarning($"[ScriptableObjectDatabase] �ߺ��� ID �߰�: {asset.ID} ({asset.DisplayName})");
            }

        });

        await handle.Task; // ��� �ε� �Ϸ� ���

        cachedValues = new T[_data.Count];
        _data.Values.CopyTo(cachedValues, 0); // ��� ĳ��
        Debug.Log($"[ScriptableObjectDatabase] {label} ���� {_data.Count}���� {typeof(T).Name} ������ �ε� �Ϸ�");
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
