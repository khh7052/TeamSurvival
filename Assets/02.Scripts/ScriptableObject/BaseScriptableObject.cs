using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "test", menuName = "Test/ScriptableObj")]
public class BaseScriptableObject : ScriptableObject
{
    [Header("Base Info Data")]
    public int ID;
    public string DisplayName;
    public string Description;
    public Sprite Icon;
    public GameObject Prefab;
    public AssetReference AssetReference;
}
