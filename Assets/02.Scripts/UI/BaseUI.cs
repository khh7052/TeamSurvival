using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    protected virtual void Awake()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    public virtual void ShowUI()
    {
        gameObject.SetActive(true);
    }

    public virtual void ExitUI()
    {
        gameObject.SetActive(false);
    }
    protected async Task WaitManagerInitialize()
    {
        while (!GameManager.Instance.IsInitialized)
        {
            await Task.Yield();
        }
    }
}
