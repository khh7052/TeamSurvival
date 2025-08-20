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

    // UI/������ �Һ񿡼� ȣ���� ���� �޼���
    public void Heal(float v) => health.Add(v);
    public void Eat(float v) => hunger.Add(v);
    public void Drink(float v) => thirst.Add(v);
    public void RecoverStamina(float v) => stamina.Add(v);

    // �ʿ��ϸ� ������ ���ٿ� ������Ƽ�� ����
    public float HealthPct => health.GetPercentage();
    public float HungerPct => hunger.GetPercentage();
    public float ThirstPct => thirst.GetPercentage();
    public float StaminaPct => stamina.GetPercentage();
}
