using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturnObject : MonoBehaviour
{
    protected virtual void OnDisable()
    {
        ObjectPoolingManager.Instance.Return(gameObject);
    }
}
