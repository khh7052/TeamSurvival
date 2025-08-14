using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "test", menuName = "Test/ScriptableObj")]
public class BaseScriptableObject : ScriptableObject
{
    [Header("Base Info Data")]
    public int ID;
    public string DisplayName;
    public Sprite Icon;
    public GameObject Prefab;
}
