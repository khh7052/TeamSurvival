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
        UIManager.Instance.ShowUI<TestConditionUI>();
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
