using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturnObject : MonoBehaviour
{
    protected virtual void OnDisable()
    {
        if (!SingletonRegistry.IsCanCreateSingleton || ObjectPoolingManager.IsApplicationQuit)
        {
            Destroy(gameObject);
            return;
        }

        var manager = ObjectPoolingManager.Instance;
        if (manager != null && manager.IsInitialized)
            manager.Return(gameObject);
        else
            Destroy(gameObject);
    }
}
