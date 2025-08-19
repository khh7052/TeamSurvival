using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    [Header("Stats")]
    public Condition health;
    public Condition hunger;
    public Condition thirst;
    public Condition stamina;

    private void Awake()
    {
        health.Init();
        hunger.Init();
        thirst.Init();
        stamina.Init();
    }

    // UI/아이템 소비에서 호출할 편의 메서드
    public void Heal(float v) => health.Add(v);
    public void Eat(float v) => hunger.Add(v);
    public void Drink(float v) => thirst.Add(v);
    public void RecoverStamina(float v) => stamina.Add(v);

    // 필요하면 게이지 접근용 프로퍼티도 제공
    public float HealthPct => health.GetPercentage();
    public float HungerPct => hunger.GetPercentage();
    public float ThirstPct => thirst.GetPercentage();
    public float StaminaPct => stamina.GetPercentage();
}
