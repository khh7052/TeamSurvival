using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "AssetList", menuName = "AssetGroup/NewAssetList")]
public class AssetReferenceData : ScriptableObject
{
    public List<AssetData> data;
}

[Serializable]
public struct AssetData
{
    [Header("ID 값")]
    public int ID;
    [Header("프리팹 주소")]
    public string assetAdress;
    [Header("데이터 주소")]
    public string dataAdress;
    [Header("오브젝트 타입")]
    public DataType dataType;
}

public enum DataType { None, Build, Resource, Consume, Equip, Gathering, Compositive }