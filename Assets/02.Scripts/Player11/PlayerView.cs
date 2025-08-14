using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;

    [Header("Move Speed")]
    [SerializeField] private TMP_Text moveSpeedText;

    [Header("Hunger")]
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private TMP_Text hungerText;

    [Header("Thirst")]
    [SerializeField] private Slider thirstSlider;
    [SerializeField] private TMP_Text thirstText;

    private EntityModel model;

    public void Initialize(EntityModel model)
    {
        if (this.model != null)
            this.model.OnChangeStatuses -= OnChangeStatuses;

        this.model = model;

        if (this.model != null)
            this.model.OnChangeStatuses += OnChangeStatuses;

        OnChangeStatuses();
    }

    public void OnChangeStatuses()
    {
        if (model == null) return;

        //체력
        if (hpSlider != null)
        {
            hpSlider.maxValue = model.health.MaxValue;
            hpSlider.value = Mathf.Clamp(model.health.CurValue, 0f, model.health.MaxValue);
        }
        if (hpText != null)
            hpText.text = $"{Mathf.RoundToInt(model.health.CurValue)}/{Mathf.RoundToInt(model.health.MaxValue)}";

        //이동속도
        if (moveSpeedText != null)
            moveSpeedText.text = $"{model.moveSpeed:0.##}";

        //배고픔
        if (hungerSlider != null)
        {
            hungerSlider.maxValue = model.hunger.MaxValue;
            hungerSlider.value = Mathf.Clamp(model.hunger.CurValue, 0f, model.hunger.MaxValue);
        }
        if (hungerText != null)
            hungerText.text = $"{model.hunger.CurValue:0}";

        //목마름
        if (thirstSlider != null)
        {
            thirstSlider.maxValue = model.thirst.MaxValue;
            thirstSlider.value = Mathf.Clamp(model.thirst.CurValue, 0f, model.thirst.MaxValue);
        }
        if (thirstText != null)
            thirstText.text = $"{model.thirst.CurValue:0}";

        Debug.Log($"Hunger: {model.hunger.CurValue}/{model.hunger.MaxValue}");

    }

    private void OnDestroy()
    {
        if (model != null)
            model.OnChangeStatuses -= OnChangeStatuses;
    }
}
