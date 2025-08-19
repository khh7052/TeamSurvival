using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option : MonoBehaviour
{
    public GameObject optionPanel;

    private bool isOpen = false;

    //사운드 추가하면 Slider랑 넣을예정

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
