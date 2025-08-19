using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Composition Recipe", menuName = "Item/Composition/NewCompositionRecipe")]
public class CompositionRecipeData : BaseScriptableObject
{
    [SerializeField]
    public List<Recipe> recipe;
}

[Serializable]
public class Recipe
{
    public int ItemID;
    public int ItemCount;
}