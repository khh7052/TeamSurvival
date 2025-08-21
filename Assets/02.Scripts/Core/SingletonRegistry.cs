using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class SingletonRegistry
{
    private static readonly List<MonoBehaviour> allSingletons = new();
    public static bool IsCanCreateSingleton = true;
    public static void Register(MonoBehaviour singleton)
    {
        if (!allSingletons.Contains(singleton))
            allSingletons.Add(singleton);
    }

    public static void Unregister(MonoBehaviour singleton)
    {
        allSingletons.Remove(singleton);
    }

    public static async void ReleaseAll()
    {
        IsCanCreateSingleton = false;
        foreach (var s in allSingletons.ToArray())
        {
            if (s != null)
            {
                Debug.Log(s.gameObject.name);
                GameObject.Destroy(s.gameObject);
                
            }

        }
        allSingletons.Clear();

        await Task.Delay(50); // 또는 Task.Yield() 여러 번
        IsCanCreateSingleton = true;
    }
}
