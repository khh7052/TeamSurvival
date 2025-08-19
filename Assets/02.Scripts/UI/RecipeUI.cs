using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour, IPointerClickHandler
{
    [Header("완성 품")]
    public Image targetItemIcon;
    public TMP_Text targetItemText;
    public TMP_Text targetItemDesc;

    private CompositionUI originUI;
    int index;
    public bool IsAble {  get; private set; }

    [Header("재료")]
    public List<Image> sourceItemIcon = new();

    public CompositionRecipeData recipe;

    public void Initialize(CompositionRecipeData recipe, int index, CompositionUI origin)
    {
        this.recipe = recipe;
        this.index = index;
        originUI = origin;
        targetItemIcon.sprite = recipe.Icon;
        targetItemText.text = recipe.DisplayName;
        targetItemDesc.text = recipe.Description;

        for(int i = 0; i < sourceItemIcon.Count; i++) 
        {
            if(i < recipe.recipe.Count)
            {
                var Item = Factory.Instance.GetDataByID<ItemData>(recipe.recipe[i].ItemID);
                Debug.Log(Item.DisplayName);
                sourceItemIcon[i].sprite = Item.Icon;
                sourceItemIcon[i].SetActive(true);
            }
            else
            {
                sourceItemIcon[i].SetActive(false);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        originUI.selectIndex = index;
    }

    public void CheckCreatableSlot()
    {

    }
}
