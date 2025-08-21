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

        //체력
        SetFill(hpFill, model.health.CurValue, model.health.MaxValue);
        if (hpText != null)
            hpText.text = $"{Mathf.RoundToInt(model.health.CurValue)}/{Mathf.RoundToInt(model.health.MaxValue)}";

        //이동속도
        if (moveSpeedText != null)
            moveSpeedText.text = $"{model.moveSpeed.totalValue:0.##}";

        //배고픔
        SetFill(hungerFill, model.hunger.CurValue, model.hunger.MaxValue);
        if (hungerText != null) hungerText.text = $"{model.hunger.CurValue:0}";

        //목마름
        SetFill(thirstFill, model.thirst.CurValue, model.thirst.MaxValue);
        if (thirstText != null) thirstText.text = $"{model.thirst.CurValue:0}";

        //스테미너
        SetFill(staminaFill, model.stamina.CurValue, model.stamina.MaxValue);
        if (staminaText != null) staminaText.text = $"{model.stamina.CurValue:0}";

        //체온
        if (tempertureText != null) //Text.Color로 상태 표시 변경 가능
        {
            float temp = model.temperture.CurValue;
            tempertureText.text = $"{temp:F1} °C";
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

        promptText.text = target.GetPrompt(); // 아이템이 제공하는 이름/설명 표시
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
        float fadeSpeed = 1.5f; // 페이드 속도 (1.5초 정도 걸려 사라짐)
        while (c.a > 0f)
        {
            c.a -= fadeSpeed * Time.deltaTime;
            dmgIndicator.color = c;
            yield return null;
        }

        // 완전히 안 보이게만 하고 오브젝트는 꺼지지 않음
        c.a = 0f;
        dmgIndicator.color = c;
    }
}
