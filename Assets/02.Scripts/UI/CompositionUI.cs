using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CompositionUI : BaseUI
{
    public GameObject compositinoListPrefab;
    public Transform ScrollViewContentTrans;
    public CompositionRecipeData[] RecipeList;  // ���� �Ŵ��� ���� �ű⼭ �޾ƿ��� �� ��
    private List<RecipeUI> RecipeUIList = new();

    public Button CreateButton;

    public int selectIndex = -1;

    private PlayerInventory playerInventory;

    protected override async void Awake()
    {
        base.Awake();
        CreateButton.onClick.RemoveAllListeners();
        CreateButton.onClick.AddListener(OnClick);
        playerInventory = GameManager.player.inventory;

        RecipeList = await AssetDataLoader.Instance.GetDatasByType<CompositionRecipeData>(DataType.Compositive);
        for (int i = 0; i < RecipeList.Length; i++)
        {
            GameObject go = Instantiate(compositinoListPrefab, ScrollViewContentTrans);
            var ui = go.GetComponent<RecipeUI>();
            RecipeUIList.Add(ui);
            ui.Initialize(RecipeList[i], i, this);
        }

        UpdateUI();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if(playerInventory == null) playerInventory = GameManager.player.inventory;
        UpdateUI();
        playerInventory.OnChangeData += UpdateUI;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        playerInventory.OnChangeData -= UpdateUI;
    }



    public void OnClick()
    {
        if(selectIndex != -1)
        {
            if (RecipeUIList[selectIndex].isCreatable)
            {
                playerInventory.ItemCreate(RecipeList[selectIndex]);
            }            
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < RecipeUIList.Count; i++)
        {
            RecipeUIList[i].CheckCreatableSlot();
        }
    }
}
