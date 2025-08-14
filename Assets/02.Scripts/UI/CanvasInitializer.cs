using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasInitializer : MonoBehaviour
{
    private void Awake()
    {
        UIManager.Instance.RegisterCanvas(GetComponent<Canvas>());
    }
}
