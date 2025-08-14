using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    public virtual void OnEnable()
    {

    }

    public virtual void OnDisable()
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
