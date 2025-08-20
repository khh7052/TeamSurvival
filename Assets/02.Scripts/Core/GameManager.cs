using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>, IInitializableAsync
{
    public bool IsInitialized {  get; private set; }
    public static Player player;
    private void Start()
    {
        InitializeAsync();

    }
    public async void InitializeAsync()
    {
        await WaitForManagersToInitialize(
            Factory.Instance,
            BuildingManager.Instance,
            GatheringManager.Instance,
            UIManager.Instance
        );
        IsInitialized = true;
        Debug.Log("[GameManager] ��� �Ŵ��� �ʱ�ȭ �Ϸ�");
        GameStart();
    }

    public async Task WaitInitializedAsync()
    {
        while (!IsInitialized)
        {
            await Task.Yield();
        }
    }

    private async Task WaitForManagersToInitialize(params IInitializableAsync[] managers)
    {
        // ��� �Ŵ����� IsInitialized == true�� �� ������ ���
        while (managers.Any(m => !m.IsInitialized))
        {
            await Task.Yield(); // ���� ������ ���
        }

    }

    public void GameStart()
    {

        /* Test Area
        UIManager.Instance.ShowUI<TestConditionUI>();
        Factory.Instance.CreateByID<BaseScriptableObject>(0);

        for(int i = 0; i < 10; i++)
        {
            await Factory.Instance.CreateByIDAsync<BaseScriptableObject>(0);
        }
        for (int i = 0; i < 10; i++)
        {
            await Factory.Instance.CreateByIDAsync<BaseScriptableObject>(1);
        }
        for (int i = 0; i < 10; i++)
        {
            await Factory.Instance.CreateByIDAsync<BaseScriptableObject>(2);
        }

        Debug.Log("Start Delay");
        await Task.Delay(5000);

        Debug.Log("End Delay");
//        await test();
        */
    }


    public void PauseGame()
    {

    }

    public void ResumeGame()
    {

    }

    public void PlayerDead()
    {

    }

    public void LoadScene(string sceneNamae)
    {

    }

}
