using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Factory : Singleton<Factory>, IInitializableAsync
{
    private ScriptableObjectDataBase<BaseScriptableObject> database;

    public bool IsInitialized { get; private set; }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeAsync();
    }

    public GameObject CreateByID<T>(int id, Action<GameObject> callBack = null) where T : BaseScriptableObject
    {
        T data = database.GetById(id) as T;
        if(data == null || data.Prefab == null)
        {
            Debug.LogError($"Factory : ID : {id} 해당 데이터가 없거나 Prefab이 없습니다");
            return null;
        }

        GameObject go = ObjectPoolingManager.Instance.Get(data.Prefab, Vector3.zero);
        go.name = data.DisplayName;
        callBack?.Invoke(go);
        return go;
    }

    public async Task<GameObject> CreateByIDAsync<T>(int id, Action<GameObject> callBack = null) where T : BaseScriptableObject
    {
        T data = database.GetById(id) as T;
        if(data == null || data.AssetReference == null)
        {
            Debug.LogError($"Factory : ID : {id} 해당 데이터가 없거나 Prefab이 없습니다");
            return null;
        }

        GameObject go = await ObjectPoolingManager.Instance.GetAsync(data.AssetReference, Vector3.zero, Quaternion.identity);
        go.name = data.DisplayName;
        if(go != null)
            callBack?.Invoke(go);
        return go;
    }

    public async void InitializeAsync()
    {
        database = new ScriptableObjectDataBase<BaseScriptableObject>();
        await database.Initialize("ItemData");
        IsInitialized = true;
        
    }
}
