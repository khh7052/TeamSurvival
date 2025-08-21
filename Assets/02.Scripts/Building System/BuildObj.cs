using System;
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

    public Action OnHitEvent { get; set; }

    public MeshRenderer mesh;
    private Coroutine coroutine;

    public void Initialize()
    {
        animationHandler = GetComponent<AnimationHandler>();
        Health.OnChanged -= Health_OnChanged;
        Health.Init();
        OnHitEvent += OnHit;
        Health.OnChanged += Health_OnChanged;

    }

    public void TakePhysicalDamage(int damage)
    {
        Health.Subtract(damage);
        OnHitEvent?.Invoke();
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
        OnHitEvent -= OnHit;
    }

    public void OnHit()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(SetMeshColorAtDamage());
    }

    private IEnumerator SetMeshColorAtDamage()
    {
        Color startColor = Color.red;       // ���� �� (����)
        Color endColor = Color.white;       // ���� �� (�Ͼ�)
        float duration = 1.0f;              // ȿ�� ���� �ð� (1��)
        float elapsed = 0f;


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; // 0 �� 1 �� ����
            mesh.material.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        // ����: ���������� ��� Ȯ��
        mesh.material.color = endColor;
    }
}
