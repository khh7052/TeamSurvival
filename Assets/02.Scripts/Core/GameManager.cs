using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>, IInitializableAsync
{
    public bool IsInitialized { get; private set; } = false;
    public static Player player;

    protected override void Awake()
    {
        base.Awake();
        IsInitialized = false;
        InitializeAsync();
        SingletonRegistry.IsCanCreateSingleton = true;
    }


    public async void InitializeAsync()
    {
        await WaitForManagersToInitialize(
            UIManager.Instance
        );
        IsInitialized = true;
        Debug.Log("[GameManager] 모든 매니저 초기화 완료");
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
        // 모든 매니저가 IsInitialized == true가 될 때까지 대기
        while (managers.Any(m => !m.IsInitialized))
        {
            await Task.Yield(); // 다음 프레임 대기
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

        SingletonRegistry.ReleaseAll();
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public async void LoadScene(string sceneNamae)
    {
        await Task.Yield();
        SceneManager.LoadSceneAsync(sceneNamae).completed += (op) =>
        {
        };
    }

}
