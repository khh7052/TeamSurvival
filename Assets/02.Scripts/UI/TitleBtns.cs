using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleBtns : BaseUI
{
    [Header("Button Setting")]
    public Button startBtn;
    public Button acieveBtn;
    public Button optionBtn;
    public Button exitBtn;

    StartBtn startLogic;
    Option optionLogic;

    private void Awake()
    {
        if(startBtn != null)
        {
            startLogic = startBtn.GetComponent<StartBtn>();
            startBtn.onClick.AddListener(OnClickStartBtn);
        }

        if(acieveBtn != null)
        {
            acieveBtn.onClick.AddListener(OnClickAchieveBtn);
        }

        if(optionBtn != null)
        {
            optionLogic = optionBtn.GetComponent<Option>();
            optionBtn.onClick.AddListener(OnClickOptionBtn);
        }
        if(exitBtn != null)
        {
            exitBtn.onClick.AddListener(OnClickExitBtn);
        }
    }

    private void OnClickStartBtn()
    {
        startLogic.GoMainScene(1f); //Scene이름 추가예정
    }

    private void OnClickAchieveBtn()
    {
        Debug.Log("도전과제 버튼 활성화");
    }

    private void OnClickOptionBtn()
    {
        optionLogic.ToggleOptionpanel();
    }

    private void OnClickExitBtn()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }
}
