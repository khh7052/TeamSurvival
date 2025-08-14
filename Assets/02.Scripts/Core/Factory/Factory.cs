using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : Singleton<Factory> 
{
    private ScriptableObjectDataBase<BaseScriptableObject> database;

    protected override void Initialize()
    {
        base.Initialize();
        database = new ScriptableObjectDataBase<BaseScriptableObject>("Data");
    }

    public GameObject CreateByID<T>(int id) where T : BaseScriptableObject
    {
        T data = database.GetById(id) as T;
        if(data == null || data.Prefab == null)
        {
            Debug.LogError($"Factory : ID : {id} 해당 데이터가 없거나 Prefab이 없습니다");
            return null;
        }

        GameObject go = ObjectPoolingManager.Instance.Get(data.Prefab, Vector3.zero);
        go.name = data.DisplayName;
        return go;
    }
}
