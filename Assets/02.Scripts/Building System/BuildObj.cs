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
    }
}
