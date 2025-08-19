using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyUI : BaseUI
{
    public DayCycle daycycle;
    public Image fillImage;

    private void Update()
    {
        fillImage.fillAmount = daycycle.time;
    }
}
