using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositionDataManager : Singleton<CompositionDataManager>, IInitializableAsync
{
    private ScriptableObjectDataBase<CompositionRecipeData> _database = new();
    public bool IsInitialized { get; private set; }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeAsync();
    }

    public async void InitializeAsync()
    {
        await _database.Initialize("CompositeRecipe");
        IsInitialized = true;
    }


}
