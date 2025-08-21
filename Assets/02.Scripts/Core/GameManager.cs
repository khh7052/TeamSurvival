using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>, ISingletonResetData
{
    public bool IsInitialized { get; private set; } = false;
    public static Player player;

    protected override void Awake()
    {
        base.Awake();
        IsInitialized = true;
        SingletonRegistry.IsCanCreateSingleton = true;
    }

    public void ResetData()
    {
        IsInitialized = false;

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
        IsInitialized = false;
        await Task.Yield();

        SceneManager.LoadSceneAsync(sceneNamae).completed += (op) =>
        {
            IsInitialized = true;
        };
    }

}
