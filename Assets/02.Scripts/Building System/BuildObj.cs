using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildObj : MonoBehaviour, IDamageable
{
    public Condition Health = new();
    public AnimationHandler animationHandler;

    [SerializeField]
    public BuildKey key;

    public bool IsDead => Health.CurValue <= 0;


    public void Initialize()
    {
        animationHandler = GetComponent<AnimationHandler>();
        Health.OnChanged -= Health_OnChanged;
        Health.Init();

        Health.OnChanged += Health_OnChanged;

    }

    public void TakePhysicalDamage(int damage)
    {
        Health.Subtract(damage);
    }

    private void Health_OnChanged()
    {
        if (IsDead)
        {
            Debug.Log("�ֵ���?");
            gameObject.SetActive(false);
        }
        else
        {
            // NPC�� �������� �޾��� ���� ����
            // (���� ������ �޾��� ���� �̺�Ʈ�� ���� �����Ƿ� �ϴ� OnChanged���� ó����, ���߿� �����ʿ�)
            animationHandler?.PlayHit();
        }
    }

    private void OnDisable()
    {
        BuildingManager.Instance?.UnregisterBuild(key);
    }
}
