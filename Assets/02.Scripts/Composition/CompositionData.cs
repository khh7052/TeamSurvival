using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Composition Recipe", menuName = "Item/Composition/NewCompositionRecipe")]
public class CompositionRecipeData : BaseScriptableObject
{
    [SerializeField]
    public List<CompositionRecipe> recipe;
}

[Serializable]
public class CompositionRecipe
{
    public int ItemID;
    public int ItemCount;
}