using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public void Initialize()
    {
        UIManager.Instance.ShowUI<InGameUI>();
    }
}