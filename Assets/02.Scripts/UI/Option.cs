using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option : MonoBehaviour
{
    public GameObject optionPanel;

    private bool isOpen = false;

    //���� �߰��ϸ� Slider�� ��������

    private void Awake()
    {
        if(optionPanel != null )
        {
            optionPanel.SetActive(false);
        }  
    }

    public void ToggleOptionpanel()
    {
        isOpen = !isOpen;
        optionPanel.SetActive(isOpen);
    }
}
