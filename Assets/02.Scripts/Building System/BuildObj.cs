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
            Debug.Log("왜뒤짐?");
            gameObject.SetActive(false);
        }
        else
        {
            // NPC가 데미지를 받았을 때의 로직
            // (현재 데미지 받았을 때의 이벤트가 따로 없으므로 일단 OnChanged에서 처리함, 나중에 수정필요)
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
        Color startColor = Color.red;       // 시작 색 (빨강)
        Color endColor = Color.white;       // 최종 색 (하양)
        float duration = 1.0f;              // 효과 지속 시간 (1초)
        float elapsed = 0f;


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; // 0 → 1 로 증가
            mesh.material.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        // 보정: 최종적으로 흰색 확정
        mesh.material.color = endColor;
    }
}
