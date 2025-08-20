using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Composition Recipe", menuName = "Item/Composition/NewCompositionRecipe")]
public class CompositionRecipeData : BaseScriptableObject
{
    [SerializeField]
    public List<Recipe> recipe;

    public (int[], int[]) GetRecipeData()
    {
        int[] datas = new int[recipe.Count];

        int[] coutns = new int[recipe.Count];

        for (int i = 0; i < datas.Length; i++)
        {
            datas[i] = recipe[i].ItemID;
            coutns[i] = recipe[i].ItemCount;
        }

        return (datas, coutns);
    }
}

[Serializable]
public class Recipe
{
    public int ItemID;
    public int ItemCount;
}