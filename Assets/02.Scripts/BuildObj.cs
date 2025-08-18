using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObj : MonoBehaviour
{
    public EntityModel model;
    public AnimationHandler animationHandler;

    [SerializeField]
    public BuildKey key;

    public bool IsDead => model.health.CurValue <= 0;

    private void Awake()
    {
        model = GetComponent<EntityModel>();
        animationHandler = GetComponent<AnimationHandler>();

        model.health.OnChanged += Health_OnChanged;
    }

    private void Health_OnChanged()
    {
        if (IsDead)
        {
            Destroy(gameObject);
        }
        else
        {
            // NPC�� �������� �޾��� ���� ����
            // (���� ������ �޾��� ���� �̺�Ʈ�� ���� �����Ƿ� �ϴ� OnChanged���� ó����, ���߿� �����ʿ�)
            animationHandler?.PlayHit();
        }
    }

    private void OnDestroy()
    {
        BuildingManager.Instance?.UnregisterBuild(key);
    }
}
