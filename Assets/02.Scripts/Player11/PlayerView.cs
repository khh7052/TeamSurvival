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

        //ü��
        if (hpSlider != null)
        {
            hpSlider.maxValue = model.maxHP;
            hpSlider.value = Mathf.Clamp(model.currentHP, 0f, model.maxHP);
        }
        if (hpText != null)
            hpText.text = $"{Mathf.RoundToInt(model.currentHP)}/{Mathf.RoundToInt(model.maxHP)}";

        //�̵��ӵ�
        if (moveSpeedText != null)
            moveSpeedText.text = $"{model.moveSpeed:0.##}";

        //�����
        if (hungerSlider != null)
        {
            hungerSlider.maxValue = model.maxHunger;
            hungerSlider.value = Mathf.Clamp(model.hunger, 0f, model.maxHunger);
        }
        if (hungerText != null)
            hungerText.text = $"{model.hunger:0}";

        //�񸶸�
        if (thirstSlider != null)
        {
            thirstSlider.maxValue = model.maxThirst;
            thirstSlider.value = Mathf.Clamp(model.thirst, 0f, model.maxThirst);
        }
        if (thirstText != null)
            thirstText.text = $"{model.thirst:0}";
    }

    private void OnDestroy()
    {
        if (model != null)
            model.OnChangeStatuses -= OnChangeStatuses;
    }
}
