using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    private void Start()
    {
        GameStart();
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
