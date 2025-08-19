using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    Button closeButton;

    protected virtual void Awake()
    {
        transform.Find("closeButton");
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
}
