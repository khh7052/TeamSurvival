using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturnObject : MonoBehaviour
{
    protected virtual void OnDisable()
    {
        if(ObjectPoolingManager.Instance != null)
            ObjectPoolingManager.Instance.Return(gameObject);
        else
        {
            Destroy(gameObject);
        }
    }
}
