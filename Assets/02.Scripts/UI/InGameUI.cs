using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : BaseUI
{
    [Header("HP")]
    [SerializeField] private Image hpFill;
    [SerializeField] private TMP_Text hpText;

    [Header("Move Speed")]
    [SerializeField] private TMP_Text moveSpeedText;

    [Header("Hunger")]
    [SerializeField] private Image hungerFill;
    [SerializeField] private TMP_Text hungerText;

    [Header("Thirst")]
    [SerializeField] private Image thirstFill;
    [SerializeField] private TMP_Text thirstText;

    [Header("Stamina")]
    [SerializeField] private Image staminaFill;
    [SerializeField] private TMP_Text staminaText;

    [Header("Temperture")]
    [SerializeField] private TMP_Text tempertureText;

    [Header("Building UI")]
    [SerializeField] private BuildSlotUI[] buildSlotUI;

    [Header("DamageIndicator")]
    [SerializeField] private Image dmgIndicator;
    private Coroutine coroutine;

    [SerializeField]
    private TMP_Text promptText;

    EntityModel model;

    protected override async void OnEnable()
    {
        base.OnEnable();
        await WaitManagerInitialize();

        model = GameManager.player.model;
        foreach(var c in model.AllConditions)
        {
            c.OnChanged += OnChangeStatuses;
        }
        model.OnHitEvent += OnDamageEvent;
        model.moveSpeed.OnChangeValue += OnChangeStatuses;
        model.jumpPower.OnChangeValue += OnChangeStatuses;
    }

    protected override void OnDisable()
    {
        base.OnDisable(); 
        foreach (var c in model.AllConditions)
        {
            c.OnChanged -= OnChangeStatuses;
        }
        model.OnHitEvent -= OnDamageEvent;
        model.moveSpeed.OnChangeValue -= OnChangeStatuses;
        model.jumpPower.OnChangeValue -= OnChangeStatuses;
    }

    public void OnChangeStatuses()
    {
        if (model == null) return;

        //ü��
        SetFill(hpFill, model.health.CurValue, model.health.MaxValue);
        if (hpText != null)
            hpText.text = $"{Mathf.RoundToInt(model.health.CurValue)}/{Mathf.RoundToInt(model.health.MaxValue)}";

        //�̵��ӵ�
        if (moveSpeedText != null)
            moveSpeedText.text = $"{model.moveSpeed.totalValue:0.##}";

        //�����
        SetFill(hungerFill, model.hunger.CurValue, model.hunger.MaxValue);
        if (hungerText != null) hungerText.text = $"{model.hunger.CurValue:0}";

        //�񸶸�
        SetFill(thirstFill, model.thirst.CurValue, model.thirst.MaxValue);
        if (thirstText != null) thirstText.text = $"{model.thirst.CurValue:0}";

        //���׹̳�
        SetFill(staminaFill, model.stamina.CurValue, model.stamina.MaxValue);
        if (staminaText != null) staminaText.text = $"{model.stamina.CurValue:0}";

        //ü��
        if (tempertureText != null) //Text.Color�� ���� ǥ�� ���� ����
        {
            float temp = model.temperture.CurValue;
            tempertureText.text = $"{temp:F1} ��C";
            if (temp >= 36f) tempertureText.color = Color.green;
            else if (temp >= 34f) tempertureText.color = Color.yellow;
            else tempertureText.color = Color.red;
        }
    }

    private static void SetFill(Image image, float cur, float max)
    {
        if (image == null) return;

        if (image.type != Image.Type.Filled)
        {
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
        }

        image.fillAmount = (max > 0f) ? Mathf.Clamp01(cur / max) : 0f;
    }

    public void SetPromptText(IInteractable target)
    {
        if (promptText == null || target == null) return;

        promptText.text = target.GetPrompt(); // �������� �����ϴ� �̸�/���� ǥ��
        promptText.gameObject.SetActive(true);
    }

    public void EndPromptText()
    {
        promptText.gameObject.SetActive(false);
    }

    public void OnDamageEvent()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(DamageIndicator());
    }

    public IEnumerator DamageIndicator()
    {
        Color c = Color.red;
        c.a = 0.6f;
        dmgIndicator.color = c;

        dmgIndicator.SetActive(true);
        float fadeSpeed = 1.5f; // ���̵� �ӵ� (1.5�� ���� �ɷ� �����)
        while (c.a > 0f)
        {
            c.a -= fadeSpeed * Time.deltaTime;
            dmgIndicator.color = c;
            yield return null;
        }

        // ������ �� ���̰Ը� �ϰ� ������Ʈ�� ������ ����
        c.a = 0f;
        dmgIndicator.color = c;
    }
}
