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
        /* Test Area
        UIManager.Instance.ShowUI<TestConditionUI>();

        for(int i = 0; i < 10; i++)
        {
            GameObject go = ObjectPoolingManager.Instance.Get(Resources.Load<GameObject>("Test/obj1"), Vector3.zero);
        }
        for (int i = 0; i < 10; i++)
        {
            ObjectPoolingManager.Instance.Get(Resources.Load<GameObject>("Test/obj2"), Vector3.zero);
        }
        for (int i = 0; i < 10; i++)
        {
            ObjectPoolingManager.Instance.Get(Resources.Load<GameObject>("Test/obj3"), Vector3.zero);
        }
        */
        Invoke("test", 3f);
    }

    public void test()
    {
        Factory.Instance.CreateByID<BaseScriptableObject>(0);
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
