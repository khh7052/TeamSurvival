using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CompositionUI : BaseUI
{
    public GameObject compositinoListPrefab;
    public Transform ScrollViewContentTrans;
    public List<CompositionRecipeData> RecipeList;  // ���� �Ŵ��� ���� �ű⼭ �޾ƿ��� �� ��
    private List<RecipeUI> RecipeUIList = new();

    public Button CreateButton;

    public int selectIndex = -1;

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
            ui.CheckCreatableSlot();
        }

        CreateButton.onClick.RemoveAllListeners();
        CreateButton.onClick.AddListener(OnClick);
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
            Debug.Log($"{selectIndex} ��ȣ ���� ����. {RecipeUIList[selectIndex].recipe.DisplayName} ���� �õ�");
        }
    }
}
