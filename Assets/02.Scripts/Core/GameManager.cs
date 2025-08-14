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

    public void GameStart()
    {

        Factory.Instance.CreateByID<BaseScriptableObject>(0);
        // Test Area
        UIManager.Instance.ShowUI<TestConditionUI>();

        for(int i = 0; i < 10; i++)
        {

            Factory.Instance.CreateByID<BaseScriptableObject>(0);
        }
        for (int i = 0; i < 10; i++)
        {
            Factory.Instance.CreateByID<BaseScriptableObject>(1);
        }
        for (int i = 0; i < 10; i++)
        {
            Factory.Instance.CreateByID<BaseScriptableObject>(2);
        }
        
        Invoke("test", 3f);
    }

    public void test()
    {
        for (int i = 0; i < 5; i++)
        {

            Factory.Instance.CreateByID<BaseScriptableObject>(0);
        }
        for (int i = 0; i < 5; i++)
        {
            Factory.Instance.CreateByID<BaseScriptableObject>(1);
        }
        for (int i = 0; i < 5; i++)
        {
            Factory.Instance.CreateByID<BaseScriptableObject>(2);
        }
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
