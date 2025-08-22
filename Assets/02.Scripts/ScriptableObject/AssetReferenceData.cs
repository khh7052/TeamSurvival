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
    [Header("ID ��")]
    public int ID;
    [Header("������ �ּ�")]
    public string assetAdress;
    [Header("������ �ּ�")]
    public string dataAdress;
    [Header("������Ʈ Ÿ��")]
    public DataType dataType;
}

public enum DataType { None, Build, Resource, Consume, Equip, Gathering, Compositive }