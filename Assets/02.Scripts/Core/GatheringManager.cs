using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GatheringManager : Singleton<GatheringManager>, IInitializableAsync
{
    private ScriptableObjectDataBase<BaseScriptableObject> _dataBase = new();

    public bool IsInitialized { get; private set; }


    protected override void Initialize()
    {
        base.Initialize();
        InitializeAsync();
    }

    public async void InitializeAsync()
    {
        await _dataBase.Initialize("Gathering");
        IsInitialized = true;
    }


    public T GetGatheringObjectData<T>(int id) where T : BaseScriptableObject
    {
        T data = _dataBase.GetById(id) as T;

        return data;
    }

    public T GetRandomObjectData<T>() where T : BaseScriptableObject
    {
        return _dataBase.GetRandomData() as T;
    }
}
