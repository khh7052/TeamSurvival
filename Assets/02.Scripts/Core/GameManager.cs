using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    private async void Start()
    {
        await WaitForManagersToInitialize(
            Factory.Instance,
            ObjectPoolingManager.Instance

        );

        Debug.Log("[GameManager] ��� �Ŵ��� �ʱ�ȭ �Ϸ�");
        GameStart();

    }

    private async Task WaitForManagersToInitialize(params IInitializableAsync[] managers)
    {
        // ��� �Ŵ����� IsInitialized == true�� �� ������ ���
        while (managers.Any(m => !m.IsInitialized))
        {
            await Task.Yield(); // ���� ������ ���
        }
    }

    public async void GameStart()
    {

        /* Test Area
        Factory.Instance.CreateByID<BaseScriptableObject>(0);
        UIManager.Instance.ShowUI<TestConditionUI>();

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
        await test();*/
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
