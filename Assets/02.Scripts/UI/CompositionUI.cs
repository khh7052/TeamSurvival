using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CompositionUI : BaseUI
{
    public GameObject compositinoListPrefab;
    public Transform ScrollViewContentTrans;
    public List<CompositionRecipeData> RecipeList;  // 따로 매니저 만들어서 거기서 받아오게 할 것
    private List<RecipeUI> RecipeUIList = new();

    public Button CreateButton;

    public int selectIndex = -1;

    private PlayerInventory playerInventory;

    protected override async void Awake()
    {
        base.Awake();
        await WaitManagerInitialize();

        for (int i = 0; i < RecipeList.Count; i++)
        {
            GameObject go = Instantiate(compositinoListPrefab, ScrollViewContentTrans);
            var ui = go.GetComponent<RecipeUI>();
            RecipeUIList.Add(ui);
            ui.Initialize(RecipeList[i], i, this);
        }

        CreateButton.onClick.RemoveAllListeners();
        CreateButton.onClick.AddListener(OnClick);
        playerInventory = GameManager.player.inventory;
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

    async Task WaitManagerInitialize()
    {
        while (!GameManager.Instance.IsInitialized)
        {
            await Task.Yield();
        }
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
