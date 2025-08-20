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

    [Header("데이터")]
    public CompositionRecipeData recipe;

    [Header("활성화 여부")]
    public bool isCreatable;
    private Image LayerImage;

    public void Initialize(CompositionRecipeData recipe, int index, CompositionUI origin)
    {
        this.recipe = recipe;
        this.index = index;
        originUI = origin;
        targetItemIcon.sprite = recipe.Icon;
        targetItemText.text = recipe.DisplayName;
        targetItemDesc.text = recipe.Description;
        LayerImage = GetComponent<Image>();

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
        LayerImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        originUI.selectIndex = index;
    }

    public void CheckCreatableSlot()
    {
        var datas = GetRecipeData();
        if(GameManager.player.inventory.IsHasItem(datas.Item1, datas.Item2))
        {
            LayerImage.color = Color.white;
            isCreatable = true;
        }
        else
        {
            LayerImage.color = Color.gray;
            isCreatable = false;
        }
    }

    private (int[], int[]) GetRecipeData()
    {
        int[] datas = new int[recipe.recipe.Count];

        int[] coutns = new int[recipe.recipe.Count];

        for(int i = 0; i < datas.Length; i++)
        {
            datas[i] = recipe.recipe[i].ItemID;
            coutns[i] = recipe.recipe[i].ItemCount;
        }

        return (datas, coutns);
    }
}
