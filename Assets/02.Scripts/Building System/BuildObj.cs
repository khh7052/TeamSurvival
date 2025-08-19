using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildObj : MonoBehaviour
{
    public EntityModel model;
    public AnimationHandler animationHandler;

    [SerializeField]
    public BuildKey key;

    public bool IsDead => model.health.CurValue <= 0;


    public void Initialize()
    {
        model = GetComponent<EntityModel>();
        animationHandler = GetComponent<AnimationHandler>();
        model.health.OnChanged -= Health_OnChanged;

        model.health.Init();

        model.health.OnChanged += Health_OnChanged;

    }

    private void Health_OnChanged()
    {
        if (IsDead)
        {
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
