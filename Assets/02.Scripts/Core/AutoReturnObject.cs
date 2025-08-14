using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturnObject : MonoBehaviour
{
    protected virtual void OnDisable()
    {
        var pool = ObjectPoolingManager.Instance;
        if (pool == null || pool._quitting) return;

        pool.Return(gameObject);
    }
}
